using DynamicData;
using ReactiveUI;
using System;
using System.Reactive.Linq;

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