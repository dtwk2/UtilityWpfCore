﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class ReflectionHelper
    {
        public static bool IsReadOnly(this PropertyInfo prop)
        {
            ReadOnlyAttribute? attrib = Attribute.GetCustomAttribute(prop, typeof(ReadOnlyAttribute)) as ReadOnlyAttribute;
            bool ro = !prop.CanWrite || (attrib != null && attrib.IsReadOnly);
            return ro;
        }

        public static IEnumerable<T?> TypesOf<T>(this IEnumerable<Assembly> assemblies) where T : class
        {
            return from type in assemblies.AllTypes()
                   where typeof(T).IsAssignableFrom(type) && !type.IsAbstract
                   select Activator.CreateInstance(type) as T;
        }


        public static IEnumerable<TypeInfo> AllTypes(this IEnumerable<Assembly> assembliesToScan)
        {
            return assembliesToScan
                .SelectMany(a => a.DefinedTypes);
        }


        public static IEnumerable RecursivePropertyValues(object e, string path)
        {
            List<IEnumerable> lst = new List<IEnumerable>();
            lst.Add(new[] { e });
            try
            {
                var xx = UtilityHelper.PropertyHelper.GetPropertyValue<IEnumerable>(e, path);
                foreach (var x in xx)
                    lst.Add(RecursivePropertyValues(x, path));
            }
            catch (Exception ex)
            {
                //
            }
            return lst.SelectMany(a => a.Cast<object>());
        }
        public static IObservable<Assembly> SelectAssemblies()
        {
            return Observable.Create<Assembly>(obs =>
            {
                var dis1 = LoadedAssemblies().Subscribe(obs);
                var dis2 = Task.Run(() => GetAssemblies())
                .ToObservable()
                .Subscribe(asses =>
                {
                    foreach (var ass in asses)
                    {
                        obs.OnNext(ass);
                    }
                });
                return new CompositeDisposable(dis1, dis2);
            });
        }

        public static IObservable<Assembly> LoadedAssemblies()
        {
            return Observable
                .FromEventPattern<AssemblyLoadEventHandler, AssemblyLoadEventArgs>(
                a => AppDomain.CurrentDomain.AssemblyLoad += a,
                a => AppDomain.CurrentDomain.AssemblyLoad -= a)
                .Select(a => a.EventArgs.LoadedAssembly);
        }

        ///<summary>
        /// <a href="https://dotnetcoretutorials.com/2020/07/03/getting-assemblies-is-harder-than-you-think-in-c/"></a> 
        /// </summary>
        public static IEnumerable<Assembly> GetAssemblies(Predicate<AssemblyName>? predicate = null)
        {
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            if (Assembly.GetEntryAssembly() is { } ass)
            {
                assembliesToCheck.Enqueue(ass);
            }

            while (assembliesToCheck.Any())
            {
                foreach (var (reference, assembly) in from reference in assembliesToCheck.Dequeue().GetReferencedAssemblies()
                                                      where (predicate?.Invoke(reference) ?? true) && loadedAssemblies.Contains(reference.FullName) == false
                                                      let assembly = Assembly.Load(reference)
                                                      select (reference, assembly))
                {
                    assembliesToCheck.Enqueue(assembly);
                    loadedAssemblies.Add(reference.FullName);
                    yield return assembly;
                }
            }
        }

        public static IEnumerable<Assembly> GetSolutionAssemblies()
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
            return assemblies;
        }
    }
}
