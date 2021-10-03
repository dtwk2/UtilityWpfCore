using System;
using System.Windows;
using UtilityWpf.Attached;
using System.Reactive.Linq;

namespace UtilityWpf
{
    public static class ObservableHelper
    {
        public static IObservable<T> Observable<T>(this IObservable<AttachedPropertyChange> observable, Predicate<DependencyObject> predicateObject, Predicate<DependencyProperty> predicateProperty)
        {
            return observable
                .Where(a => predicateObject(a.d))
                .Where(a => predicateProperty(a.e.Property))
                .Select(a => a.e.NewValue)
                .Cast<T>();
        } 
    }
}