using UtilityInterface.Generic;

namespace UtilityWpf
{
    public interface IDelayedConstructorService<R> : IDelayedConstructor, IService<R>
    {
        //R Method(T input);
    }
}