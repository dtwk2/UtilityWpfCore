using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace UtilityWpf
{
    public static class ObservableCollectionHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }

        public static IObservable<NotifyCollectionChangedEventArgs> SelectChanges(this INotifyCollectionChanged oc)
        {
            return Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => oc.CollectionChanged += h,
                h => oc.CollectionChanged -= h)
                .Select(_ => _.EventArgs);
        }

        public static IObservable<object> MakeObservable(this IEnumerable oc)
        {

            var subject = new Subject<object>();
            //foreach (var o in oc.Cast<object>())
            //    subject.OnNext(o);

            if (oc is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged
                    .SelectNewItems<object>()
                    .Subscribe(a =>
                    {
                        subject.OnNext(a);
                    });

            return oc.Cast<object>().ToObservable().Concat(subject);
        }




        public static IObservable<T> SelectNewItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .SelectMany(x => x.NewItems?.Cast<T>() ?? new T[] { });

        }

        public static IObservable<T> SelectOldItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .SelectMany(x => x.OldItems?.Cast<T>() ?? new T[] { });
        }

        public static IObservable<NotifyCollectionChangedAction> SelectActions(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .Select(x => x.Action);
        }



    }
}