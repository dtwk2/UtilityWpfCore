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


        public static void AddBootStrapperRegistrations(this ContainerBuilder builder, IEnumerable<Assembly>? assembliesToScan = null)
        {
            foreach (IBootStrapper? bootStrapper in BootStrappers())
            {
                bootStrapper?.Register(builder);
            }

            IEnumerable<IBootStrapper?> BootStrappers()
            {
                return (assembliesToScan ?? AssemblySingleton.Instance.Assemblies.Where(a => !a.IsDynamic)).TypesOf<IBootStrapper>();
            }
        }

     
    }


}
