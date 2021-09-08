using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ReflectionHelper
    {
        public IObservable<Assembly> SelectAssemblies()
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
