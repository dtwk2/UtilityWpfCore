using System;
using ReactiveUI;

namespace UtilityWpf.Abstract
{
    public class OutputService<T> : ReactiveObject, IOutputService<T>
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
}