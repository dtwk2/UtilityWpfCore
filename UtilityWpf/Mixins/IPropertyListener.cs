using Evan.Wpf;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using UtilityWpf.Property;

namespace UtilityWpf.Mixins
{
    public interface IPropertyListener : IDependencyObjectListener
    {
        protected NameTypeDictionary<SingleReplaySubject<object>> dict { get; }

        public void Init([CallerMemberName] string? name = null)
        {
            //if (name != nameof(FrameworkElement.OnApplyTemplate))
            //{
            //    throw new Exception("Init must be called isnide OnApplyTemplate");
            //}

            var dependencyProperties = GetType().SelectDependencyProperties().TakeWhile(a => a.OwnerType.FullName.StartsWith("System") == false).ToArray();

            foreach (var dp in dependencyProperties)
            {
                if (dp.Name == "Id")
                {
                    dp.AddValueChanged(this.AsDependencyObject(), (sender, _) => dict[dp.Name].OnNext(this.AsDependencyObject().GetValue(dp)));
                }
            }

            foreach (var property in dependencyProperties)
            {
                dict[property.Name].OnNext((this.AsDependencyObject()?.GetValue(property)));
            }

            //foreach (var (key, subject) in dict)
            //{
            //    var single = dependencyProperties.Where(a => a.Name == key).SingleOrDefault();
            //    if (single != null)
            //    {
            //        //if (single?.GetValue(this) is { } value)
            //        subject.OnNext((this.AsDependencyObject()?.GetValue(single)));
            //    }
            //}
        }

        public IObservable<T> Observable<T>(string? name = null)
        {
            return new Observable<T>(name == null ? dict[typeof(T)] : dict[name]);
        }

        public IObserver<T> Observer<T>(string? name = null)
        {
            return new Observer<T>(name == null ? dict[typeof(T)] : dict[name]);
        }

        public IObservable<object>? Observable(string name)
        {
            return dict[name];
        }

        public IObserver<object>? Observer(string name)
        {
            return dict[name];
        }

        //private void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    Init();
        //    string name = dependencyPropertyChangedEventArgs.Property.Name;
        //    object value = dependencyPropertyChangedEventArgs.NewValue;
        //    //await System.Threading.Tasks.Task.Run(() =>
        //    //{
        //    dict[name].OnNext(value);
        //    //});
        //}

        //public static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is IPropertyListener c)
        //        OnNext(e);
        //    else
        //        throw new Exception("b77adsf");

        //    void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //    {
        //        string name = dependencyPropertyChangedEventArgs.Property.Name;
        //        object value = dependencyPropertyChangedEventArgs.NewValue;
        //        //await System.Threading.Tasks.Task.Run(() =>
        //        //{
        //        c.dict[name].OnNext(value);
        //        //});
        //    }

        //}

        public void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            dict[e.Property.Name].OnNext(e.NewValue);

            //  base.OnPropertyChanged(e);
        }
    }

    internal class Observer<T> : IObserver<T>
    {
        private readonly ISubject<object> subject;

        public Observer(ISubject<object> subject)
        {
            this.subject = subject;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException("fsdd7777777f");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException("fsdxxxd7777777f");
        }

        public void OnNext(T value)
        {
            subject.OnNext(value);
        }
    }

    internal class Observable<T> : IObservable<T>
    {
        private readonly ISubject<object> subject;

        public Observable(ISubject<object> subject)
        {
            this.subject = subject;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Cast<T>().Subscribe(observer);
        }
    }

    public class SingleReplaySubject<T> : ISubject<T>
    {
        private readonly ReplaySubject<T> subject;

        public SingleReplaySubject()
        {
            subject = new ReplaySubject<T>(1);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}