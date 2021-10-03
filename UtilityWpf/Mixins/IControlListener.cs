using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using UtilityHelperEx;

namespace UtilityWpf.Mixins
{
    public interface IControlListener : IDependencyObjectListener
    {
        protected IObservable<FrameworkElement> lazy { get; set; }

        public IObservable<T> Control<T>(string? name = null) where T : FrameworkElement
        {
            return Observable.Create<T>(observer =>
            {
                lazy ??= Get().SelectMany();
                var dis = (name == null ?
                 lazy.OfType<T>() :
                 lazy.OfType<T>().Where(a => a.Name == name))
                 .Subscribe(a =>
                 {
                     observer.OnNext(a);
                 });
                //observer.OnCompleted();
                return dis;
            })
            .ObserveOnDispatcher()
            .SubscribeOnDispatcher();
        }

        protected IObservable<IEnumerable<FrameworkElement>> Get()
        {
            var dependencyObject = this as DependencyObject ?? throw new Exception("Expected type to DependencyObject");
            if (dependencyObject is FrameworkElement control)
            {
                if (control.IsLoaded == false)
                {
                    return Observable.Create<FrameworkElement[]>(observer =>
                    {
                        return control
                            .LoadedChanges()
                            .Subscribe(a =>
                        {
                            var t = dependencyObject
                            .FindVisualChildren<FrameworkElement>()
                            .ToArray();
                            observer.OnNext(t);
                        });
                    })
                         .ObserveOnDispatcher()
                         .SubscribeOnDispatcher()
                         .ToReplaySubject(1);

                }
            }
            return Observable
                .Return(dependencyObject
                .FindVisualChildren<FrameworkElement>()
                .ToArray());
        }
    }
}