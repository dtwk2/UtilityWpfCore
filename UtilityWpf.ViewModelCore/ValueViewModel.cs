namespace UtilityWpf.ViewModel
{
    //// conside  instead ReactiveProperty
    //public class ValueViewModel<T> : NPC, IOutputService<T>
    //{
    //    public T Value { get; private set; }

    //    //public IObservable<T> Output { get; private set; }
    //    public IObservable<T> Output { get; } = new ReactiveCommand<T>();

    //    public ValueViewModel(IObservable<T> measurements, IScheduler ui)
    //    {
    //        // measurements.Subscribe(_ =>
    //        //Console.WriteLine(_.Key));

    //        if (measurements != null)
    //            measurements.ObserveOn(ui).Subscribe(
    //                      meas =>
    //                      {
    //                          if (!meas.Equals(Value))
    //                          {
    //                              Value = meas;
    //                              OnPropertiesChanged(nameof(Value));
    //                          }
    //                      },
    //                  ex => Console.WriteLine("error in observable")
    //               , () => Console.WriteLine("Observer has unsubscribed from timed observable"));
    //        else
    //            Console.WriteLine("measurements-service equals null in collectionviewmodel");

    //        //Output = Observable.Create<T>(observer => Out.Subscribe(_ => observer.OnNext(Value)));

    //    }

    //    public ValueViewModel(IEnumerable<T> measurements)
    //    {
    //        if (measurements != null)
    //            Value = measurements.Last();
    //        else
    //            Console.WriteLine("measurements-service equals null in collectionviewmodel");

    //    }

    //}

    //public class ValueViewModel2<T> : INPCBase, IOutputViewModel<T>
    //{
    //    public T Value { get; private set; }
    //    IObservable<T> output = new Subject<T>();
    //    public IObservable<T> Output => output;
    //    public ReactiveCommand Out = new ReactiveCommand();
    //    public ButtonDefinitionsViewModel<T> ButtonsVM { get; private set; }

    //    public ValueViewModel2(IObservable<T> measurements, IObservable<KeyValuePair<string, Func<T, T>>> kvps, IScheduler ui)
    //    {
    //        // measurements.Subscribe(_ =>
    //        //Console.WriteLine(_.Key));

    //        //ButtonsVM.Output.Subscribe(__ => observer.OnNext(__))

    //        if (measurements != null)
    //            measurements.ObserveOn(ui).Subscribe(
    //                      meas =>
    //                      {
    //                          if (!meas.Equals(Value))
    //                          {
    //                              ButtonsVM = new ButtonDefinitionsViewModel<T>(kvps.Select(_ => new KeyValuePair<string, Func<T>>(_.Key, () => _.Value(meas))), ui);
    //                              //Output = Observable.Create<T>(observer => Out.Subscribe(_ => ButtonsVM.Output.Subscribe(__=> observer.OnNext(__))));
    //                              Out.WithLatestFrom(ButtonsVM.Output, (a, b) => b).Subscribe(bb => ((ISubject<T>)output).OnNext(bb));
    //                              Value = meas;
    //                              NotifyChanged(nameof(Value));
    //                              NotifyChanged(nameof(ButtonsVM));

    //                          }
    //                      },
    //                  ex => Console.WriteLine("error in observable")
    //               , () => Console.WriteLine("Observer has unsubscribed from timed observable"));
    //        else
    //            Console.WriteLine("measurements-service equals null in collectionviewmodel");

    //    }

    //    public ValueViewModel2(IEnumerable<T> measurements)
    //    {
    //        if (measurements != null)
    //            Value = measurements.Last();
    //        else
    //            Console.WriteLine("measurements-service equals null in collectionviewmodel");

    //        output= Observable.Create<T>(observer => Out.Subscribe(_ => observer.OnNext(Value)));

    //    }

    //}
}