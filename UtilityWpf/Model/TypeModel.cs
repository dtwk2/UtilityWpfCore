using MoreLinq;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilityWpf.Abstract;

namespace UtilityWpf.Model
{
    public class TypeModel : ITypeModel
    {
        private AsyncLazy<Type[]> types;

        public TypeModel(Type type)
        {
            types = new AsyncLazy<Type[]>(() => Task.Run(() => type.Assembly
            .GetTypes()
                   .Where(Filter)
            .ToArray()));
        }

        public TypeModel()
        {
            types = new AsyncLazy<Type[]>(() => Task.Run(() =>
           (Assembly.GetEntryAssembly())
            .GetTypes()
                    .Where(Filter)
            .ToArray()));
        }

        public TypeModel(string assemblyName)
        {
            types = new AsyncLazy<Type[]>(() => Task.Run(() =>
           (assemblyName != null ? Assembly.Load(assemblyName) : Assembly.GetEntryAssembly())
            .GetTypes()
                    .Where(Filter)
            .ToArray()));
        }

        public TypeModel(Type[] types)
        {
            this.types = new AsyncLazy<Type[]>(() => Task.Run(() =>
            types
            .Select(t => t.Assembly)
            .DistinctBy(a => a.FullName)
            .SelectMany(a => a.GetTypes()
                     .Where(Filter))
            .ToArray()));
        }

        public TypeModel(ISet<string> assemblyNames)
        {
            types = new AsyncLazy<Type[]>(() => Task.Run(() =>
           assemblyNames.Select(name => Assembly.Load(name))
            .DistinctBy(a => a.FullName)
            .SelectMany(a => a.GetTypes()
             // .Where(type => type.GetCustomAttribute<ViewModelAttribute>() != null))
             .Where(Filter))
            .ToArray()));
        }

        protected bool Filter(Type type)
        {
            return type.Name.EndsWith("ViewModel");
            //type.GetCustomAttribute<ViewModelAttribute>() != null;
        }

        public Task<Type[]> Collection => types.Task;
    }
}
