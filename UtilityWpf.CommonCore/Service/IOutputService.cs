using ReactiveUI;
using System;

namespace UtilityWpf
{
    public class OutputService<T> :ReactiveObject, IOutputService<T>
    {
        private ObservableAsPropertyHelper<T> output;

        public OutputService(IObservable<T> _output)
        {
            output = _output.ToProperty(this, a => a.Output);
        }

        public virtual T Output => output.Value;
    }

    public interface IOutputService<T> //: INotifyPropertyChanged
    {
        T Output { get; }
    }

    //public interface IOutputViewModel2<T> : INotifyPropertyChanged
    //{
    //    T Output { get; set; }

    //}

    //public class OutputViewModel2<T> :AbstractNotifyPropertyChanged, IOutputViewModel2<T>
    //{
    //    private T _output;

    //    public T Output
    //    {
    //        get => _output;
    //        set => SetAndRaise(ref _output, value);
    //    }
    //}

    //public static class OutputViewModelHelper2
    //{
    //    // saves having to add reference to reactive propety for dependent projects
    //    public static System.IObservable<T> OutputAsObservable<T>(this IOutputViewModel2<T> fd)
    //    {
    //        return fd.WhenValueChanged(_ => _.Output).Where(_ => _ != null);
    //    }
    //}
}