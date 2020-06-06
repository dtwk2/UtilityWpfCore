using UtilityInterface.Generic;

namespace UtilityWpf
{
    public interface IDelayedConstructorService<R> : IDelayedConstructor, IObservableService<R>
    {
        //R Method(T input);
    }
}