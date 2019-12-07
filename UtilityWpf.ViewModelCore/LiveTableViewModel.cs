using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
//using System.Windows.Data;

namespace UtilityWpf.ViewModel
{
    //public class LiveTableViewModel<T>
    //{
    //    // the items source to an observablecollection
    //    public ICollectionViewLiveShaping View { get; set; }

    //    private ObservableCollection<T> _items = new ObservableCollection<T>();

    //    public LiveTableViewModel(IObservable<T> t, string sortonproperty, ListSortDirection lsd = ListSortDirection.Descending)
    //    {
    //        View = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(_items);
    //        View.IsLiveSorting = true;
    //        ((ICollectionView)View).SortDescriptions.Add(new SortDescription(sortonproperty, lsd));

    //        t.ObserveOnDispatcher().Subscribe(_ => _items.Add(_));
    //    }

    //    public LiveTableViewModel(IObservable<T> t, IObservable<KeyValuePair<string, ListSortDirection>> sortonproperty = null, IObservable<Predicate<object>> filterbyproperty = null)
    //    {
    //        View = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(_items);
    //        View.IsLiveSorting = true;
    //        sortonproperty?.ObserveOnDispatcher().Subscribe(_ =>
    //        ((ICollectionView)View).SortDescriptions.Add(new SortDescription(_.Key, _.Value)));

    //        Predicate<object> a = null;
    //        filterbyproperty.Subscribe(_ =>
    //        {
    //            if (a != null) ((ICollectionView)View).Filter -= _;
    //            a = _;
    //            ((ICollectionView)View).Filter += _;
    //        });

    //        t.ObserveOnDispatcher().Subscribe(_ => _items.Add(_));
    //    }
    //}
}