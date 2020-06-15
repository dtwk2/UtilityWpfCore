using UtilityHelper;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Service
{
    public class ContainsFilter : IFilter
    {
        private string _a = null;
        private string _property = null;

        public ContainsFilter(string a)
        {
            _a = a;
        }

        public ContainsFilter(string a, string property = null)
        {
            _property = property;
            _a = a;
        }

        public bool Filter(object o) => _property == null ? ((string)o).Contains(_a) : o.GetPropertyValue<string>(_property).Contains(_a);
    }
}