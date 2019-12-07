namespace UtilityWpf
{
    //public abstract class InputOutputService<T,R>: IOutputService<R>
    //{
    //    public InputOutputService(IService<T> input, IDelayedConstructorService<R> f)
    //    {
    //        Output = Observable.Create<IObservable<R>>(observer =>
    //        {
    //            return f.Init(input.Resource)
    //                .ToObservable().Subscribe(_ =>
    //                {
    //                    observer.OnNext(f.Init(null));
    //                });
    //        }).Switch();
    //    }

    //    public IObservable<R> Output { get; set; }

    //}
}