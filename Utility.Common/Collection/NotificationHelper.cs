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
        public static IObservable<(T source, string? propertyName)> Changes<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Select(e => (source, e.EventArgs.PropertyName));
        }

        public static IObservable<R?> Changes<T, R>(this T source, string name) where R : class
            where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                               .Where(a => a.EventArgs.PropertyName == name)
                            .Select(_ => UtilityHelper.PropertyHelper.GetPropertyRefValue<R>(source, xx));
        }

        public static IObservable<Tuple<T, R?>> OnPropertyChangeWithSource<T, R>(this T source, string name) where R : class
    where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                                .Where(a => a.EventArgs.PropertyName == name)
                                .Select(_ => Tuple.Create(source, UtilityHelper.PropertyHelper.GetPropertyRefValue<R>(source, xx)));
        }
    }
}