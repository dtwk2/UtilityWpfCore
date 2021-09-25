using System.Collections.Generic;
using System.Reflection;

namespace Utility.Common
{
    class AssemblySingleton
    {
        private AssemblySingleton()
        {
            Assemblies = UtilityHelper.ReflectionHelper.GetAssemblies(a => a.Name?.StartsWith(Meta.Constants.GeneralAssemblyName) ?? false);
        }
        public IEnumerable<Assembly> Assemblies { get; }

        public static AssemblySingleton Instance { get; } = new AssemblySingleton();
    }
}
