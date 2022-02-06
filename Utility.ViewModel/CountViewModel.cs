using DynamicData;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using Utility.Service;

namespace Utility.ViewModel;

public class CountViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<int> count;

    public CountViewModel(IObservable<IChangeSet> changeSet)
    {
        count = changeSet
            .Scan(0, (a, b) => a + b.Adds - b.Removes)
            .ToProperty(this, a => a.Count);
    }

    public int Count => count.Value;
}

public class FilteredCountViewModel<T> : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<int> count;

    public FilteredCountViewModel(IObservable<IChangeSet<T>> changeSet, IFilterService<T> filterService)
    {
        count = changeSet
                    .Filter(filterService)
                    .Scan(0, (a, b) => a + b.Adds - b.Removes)
                    .ToProperty(this, a => a.Count);
    }

    public int Count => count.Value;
}