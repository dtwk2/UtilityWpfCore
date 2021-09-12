using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Common
{
    public interface IBootStrapper
    {
        void Register(ContainerBuilder containerBuilder);
    }

    public static class BootStrapHelper
    {
        public static void AddRegistrations(this ContainerBuilder builder, IEnumerable<Assembly>? assembliesToScan = null)
        {
            foreach (var bs in SelectBootStrappers(assembliesToScan ?? AssemblySingleton.Instance.Assemblies))
            {
                bs?.Register(builder);
            }

            static IEnumerable<IBootStrapper?> SelectBootStrappers(IEnumerable<Assembly> assembliesToScan)
            {
                return from type in SelectTypes(assembliesToScan)
                       where typeof(IBootStrapper).IsAssignableFrom(type) && !type.IsAbstract
                       select Activator.CreateInstance(type) as IBootStrapper;

                static IEnumerable<TypeInfo> SelectTypes(IEnumerable<Assembly> assembliesToScan)
                {
                    return assembliesToScan
                        .Where(a => !a.IsDynamic)
                        .SelectMany(a => a.DefinedTypes);
                }
            }
        }
    }
}
