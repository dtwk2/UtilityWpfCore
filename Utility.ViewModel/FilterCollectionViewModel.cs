using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Utility.ViewModel;

public class FilterCollectionViewModel<T> : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<CheckViewModel> filterCollection;

    public FilterCollectionViewModel(IObservable<IChangeSet<T>> changeSet, Filter<T> filter)
    {
        changeSet
            .DistinctValues(filter.Func)
            .Transform(a => new CheckViewModel(a, true))
            .Bind(out filterCollection)
            .Subscribe();

        Command = ReactiveCommand.Create<Dictionary<object, bool?>, object>(a =>
        {
            filter.OnNext(a.Where(a => a.Key != default && a.Value.HasValue).ToDictionary(a => (string)a.Key, a => a.Value!.Value));
            return a;
        });
    }
    public ICommand Command { get; }

    public ICollection FilterCollection => filterCollection;
}
