using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

namespace UtilityWpf.Mixins
{
    public interface IControlListener : IDependencyObjectListener
    {
        protected FrameworkElement[] lazy { get; set; }

        public IObservable<T> Control<T>(string? name = null) where T : FrameworkElement
        {
            return Observable.Create<T>(observer =>
            {
                lazy ??= Get();
                var dis = (name == null ?
                 lazy.OfType<T>() :
                 lazy.OfType<T>().Where(a => a.Name == name))
                 .ToObservable()
                 .Subscribe(a =>
                 {
                     observer.OnNext(a);
                 });
                //observer.OnCompleted();
                return dis;
            });
        }

        protected FrameworkElement[] Get()
        {
            return (this as DependencyObject ?? throw new Exception("Expected type to DependencyObject"))
       .FindVisualChildren<FrameworkElement>()
       .ToArray();
        }
    }
}