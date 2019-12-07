using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf
{
    public static class ObservableCollectionHelper
    {
        public static IObservable<NotifyCollectionChangedEventArgs> GetCollectionChanges<T>(this ObservableCollection<T> oc)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(h => oc.CollectionChanged += h, h => oc.CollectionChanged -= h).Select(_ => _.EventArgs);
        }
    }
}