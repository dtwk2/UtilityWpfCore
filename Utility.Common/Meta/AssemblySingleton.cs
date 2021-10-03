using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Utility.Common
{
    public class AssemblySingleton
    {
        private AssemblySingleton()
        {
            Assemblies = UtilitySolutionAssemblies().ToArray();

            static IEnumerable<Assembly> UtilitySolutionAssemblies()
            {
                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*" + Meta.Constants.GeneralAssemblyName + "*.dll").ToArray();
                return files
                    .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
            }
        }

        public IReadOnlyCollection<Assembly> Assemblies { get; }

        public static AssemblySingleton Instance { get; } = new AssemblySingleton();
    }
}
