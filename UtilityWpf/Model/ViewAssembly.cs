using System.Reflection;

namespace UtilityWpf.Model
{
    public class ViewAssembly
    {
        public ViewAssembly(Assembly assembly)
        {
            Assembly = assembly;
        }

        public string? Key => Assembly.FullName;
        public Assembly Assembly { get; }
    }
}