using System;

namespace UtilityWpf.Mixins
{
    public interface IDependencyObjectListener
    {
        protected Type Type { get; }
    }

}