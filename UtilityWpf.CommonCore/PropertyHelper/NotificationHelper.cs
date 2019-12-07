using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf
{
    public static class NotificationExtensions
    {
        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<T> OnAnyPropertyChange<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Select(_ => source);
        }

        public static IObservable<R> OnPropertyChange<T, R>(this T source, string name)
            where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                               .Where(_ => _.EventArgs.PropertyName == name)
                            .Select(_ => UtilityHelper.PropertyHelper.GetPropertyValue<R>(source, xx));
        }

        public static IObservable<Tuple<T, R>> OnPropertyChangeWithSource<T, R>(this T source, string name)
    where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return System.Reactive.Linq.Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                                .Where(_ => _.EventArgs.PropertyName == name)
                                .Select(_ => Tuple.Create(source, UtilityHelper.PropertyHelper.GetPropertyValue<R>(source, xx)));
        }
    }
}