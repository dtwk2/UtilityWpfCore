using System;
using System.Windows;

namespace UtilityWpf.Mixins
{
    public static class NameTypeDictionaryHelper
    {
        public static DependencyObject AsDependencyObject(this IDependencyObjectListener listener)
        {
            return listener as DependencyObject ?? throw new Exception("Expected type to be " + nameof(DependencyObject));
        }

        public static IObservable<T> Observable<T>(this IPropertyListener listener, string? name = null)
        {
            return listener.Observable<T>(name);
        }

        public static IObserver<T> Observer<T>(this IPropertyListener listener, string? name = null)
        {
            return listener.Observer<T>(name);
        }

        public static IObservable<T> Observer<T>(this IControlListener listener, string? name = null)
            where T : FrameworkElement
        {
            return listener.Control<T>(name);
        }

        public static IObservable<object> Observable(this IPropertyListener listener, string name)
        {
            return listener.Observable(name);
        }

        public static IObserver<object> Observer(this IPropertyListener listener, string name)
        {
            return listener.Observer(name);
        }

        public static IObservable<T> Control<T>(this IControlListener listener, string? name = null)
            where T : FrameworkElement
        {
            return listener.Control<T>(name);
        }

        //        public static IObservable<object> ObservableOfName<TObservable>(this NameTypeDictionary<TObservable> dict, string name)
        //            where TObservable : IObservable<object>, new()
        //        {
        //            return dict[name];
        //        }

        //        public static IObservable<object> ObservableOfType<TObservable>(this NameTypeDictionary<TObservable> dict, Type type)
        //              where TObservable : IObservable<object>, new()
        //        {
        //            return dict[type];
        //        }

        //        public static IObserver<object> ObserverOfName<TObservable>(this NameTypeDictionary<TObservable> dict, string name )
        //              where TObservable : IObserver<object>, new()
        //        {
        //            return  dict[name];
        //        }

        //        public static IObserver<object> Observer<TObservable>(this NameTypeDictionary<TObservable> dict, Type type)
        //      where TObservable : IObserver<object>, new()
        //{
        //            return dict[type];
        //        }




        /// <summary>
        /// Whatches for any changes to dependency properties
        /// </summary>
        /// <returns></returns>
        //public static IObservable<Dictionary<string, object>> Any<T>(this NameTypeDictionary<T> dict)
        //      where T : IObservable<object>, new()
        //{
        //    return Observable.Create<Dictionary<string, IObservable<object>>>(observer =>
        //    {
        //        //Dictionary<string, IObservable<object>> dict = new();
        //        Subject<Dictionary<string, object>> sub = new();
        //        List<IDisposable> xx = new();
        //        foreach (var (key, value) in dict)
        //        {
        //            xx.Add(value.Subscribe(a => { dict[key] = a; sub.OnNext(dict); }));
        //        }
        //        return new System.Reactive.Disposables.CompositeDisposable(xx);
        //    });
        //}
    }
}