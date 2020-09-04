using Autofac;
using MoreLinq;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Interactive.Demo.Common
{
    public class ViewModelAssemblyModel
    {
        private readonly Lazy<Type[]> types;
        private readonly AsyncLazy<TypeObject[]> typeObjects;

        public ViewModelAssemblyModel(Type type) : this()
        {
            types = new Lazy<Type[]>(() => type.Assembly
            .GetTypes()
                   .Where(Filter)
            .ToArray());
        }

        public ViewModelAssemblyModel(string assemblyName = null) : this()
        {
            types = new Lazy<Type[]>(() =>
            (assemblyName != null ? Assembly.Load(assemblyName) : Assembly.GetEntryAssembly())
            .GetTypes()
                    .Where(Filter)
            .ToArray());
        }

        public ViewModelAssemblyModel(Type[] types) : this()
        {
            this.types = new Lazy<Type[]>(() => types
            .Select(a => a.Assembly)
            .DistinctBy(a => a.FullName)
            .SelectMany(a => a.GetTypes()
            //.Where(type => type.GetCustomAttribute<ViewModelAttribute>() != null))
                     .Where(Filter))
            .ToArray());
        }

        public ViewModelAssemblyModel(ISet<string> assemblyNames) : this()
        {
            types = new Lazy<Type[]>(() => assemblyNames.Select(a => Assembly.Load(a))
            .DistinctBy(a => a.FullName)
            .SelectMany(a => a.GetTypes()
             // .Where(type => type.GetCustomAttribute<ViewModelAttribute>() != null))
             .Where(Filter))
            .ToArray());
        }

        private ViewModelAssemblyModel()
        {
            typeObjects = new AsyncLazy<TypeObject[]>(() => Task.Run(() => SelectTypeObjects()));
        }


        public void Register(ContainerBuilder containerBuilder)
        {
            foreach (var type in types.Value)
                containerBuilder.RegisterType(type);
        }

        public Task<TypeObject[]> Collection => typeObjects.Task;


        protected bool Filter(Type type)
        {
            return type.Name.EndsWith("ViewModel");
            //type.GetCustomAttribute<ViewModelAttribute>() != null;
        }

        TypeObject[] SelectTypeObjects()
        {

            var xs = types.Value
                   .SelectMany(type =>
                   {
                       var arr = Splat.Locator.Current.GetServices(type).Select(a => (a, type)).ToArray();
                       if (arr.Length == 0 && type.GetConstructor(Type.EmptyTypes) != null)
                           return new[] { (Activator.CreateInstance(type), type) };
                       return arr;
                   }).ToArray();



            return xs.Select(st =>
            {
                var (service, type) = st;
                var name = typeof(IName).IsAssignableFrom(type) ?
                                                (service as IName).Name :
                                                 type.Name;
                //return new KeyValuePair<string, KeyValuePair<string, object>>(type.Name, new KeyValuePair<string, object>(name, service));
                return new TypeObject { TypeName = type.Name, Key = name, Object = service };
            }).ToArray();
        }
    }
}
