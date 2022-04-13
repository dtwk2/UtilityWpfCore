using System.Linq;
using System.Reflection;

namespace UtilityWpf.Model
{
    public class AssemblyKeyValue : KeyValue
    {
        private readonly Assembly assembly;
        public AssemblyKeyValue(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public override string? Key => assembly.FullName.Split(",").First();
        public override Assembly Value => assembly;

        public override string ToString()
        {
            return Key ?? "No Key!";
        }
    }
}