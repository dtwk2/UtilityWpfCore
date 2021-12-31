using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Utility.ViewModel;

public class CollectionViewModel<T> : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<T> collection;

    public CollectionViewModel(IObservable<IChangeSet<T>> changeSet, Filter<T> filter)
    {
        changeSet
            .Filter(filter)
            .Select(a => a)
            .Bind(out collection)
            .Subscribe();
    }

    public ICollection Collection => collection;
}
