using MoreLinq;
using System.Linq;

namespace UtilityWpf.Model
{
    public abstract class KeyValue
    {
        public abstract string Key { get; }
        public abstract object Value { get; }

        public virtual string GroupKey => Key.Split(".").First();
    }
}