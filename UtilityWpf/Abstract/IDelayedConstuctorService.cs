using UtilityInterface.Generic;

namespace UtilityWpf.Abstract
{
    public interface IDelayedConstructorService<R> : IDelayedConstructor, IObservableService<R>
    {
        //R Method(T input);
    }
}