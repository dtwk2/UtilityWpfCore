using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Common.Model;

namespace Utility.ViewModel;

public class GroupViewModel : BaseReactiveDisposable
{
    private bool isSelected = false;

    public virtual int Count { get; }

    public virtual IEnumerable Collection { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
    }
}

public class GroupViewModel<T, TKey, TGroupKey> : GroupViewModel
{
    private int count;
    private IReadOnlyCollection<T> collection;

    public GroupViewModel(IGroup<T, TKey, TGroupKey> group)
    {
        Key = group.Key;

        group
             .Cache
             .Connect()
             .ToCollection()
             .Subscribe(a =>
             {
                 this.RaiseAndSetIfChanged(ref count, a.Count, nameof(Count));
                 this.RaiseAndSetIfChanged(ref collection, a, nameof(Collection));
             },
             e => { })
            .DisposeWith(CompositeDisposable);
    }

    public TGroupKey Key { get; private set; }

    public override int Count => count;

    public override IReadOnlyCollection<T> Collection => collection;
}

public class GroupViewModel<T, TGroupKey> : GroupViewModel
{
    private int count;
    private IReadOnlyCollection<T> collection;

    public GroupViewModel(IGroup<T, TGroupKey> group)
    {
        Key = group.GroupKey;

        group
             .List
             .Connect()
             .ToCollection()
            .Subscribe(a =>
            {
                this.RaiseAndSetIfChanged(ref count, a.Count, nameof(Count));
                this.RaiseAndSetIfChanged(ref collection, a, nameof(Collection));
            },
            e =>
            {
            })
            .DisposeWith(CompositeDisposable);
    }

    public TGroupKey Key { get; private set; }

    public override int Count => count;

    public override IReadOnlyCollection<T> Collection => collection;
}

public class Group<T, TKey, TGroupKey> : IGroup<T, TKey, TGroupKey>
{
    public Group(TGroupKey key, IObservable<IChangeSet<T, TKey>> observable)
    {
        Cache = observable.AsObservableCache();
        Key = key;
    }

    public IObservableCache<T, TKey> Cache { get; }

    public TGroupKey Key { get; }
}

public class Group<T, TGroupKey> : IGroup<T, TGroupKey>
{
    public Group(TGroupKey key, IEnumerable<T> observable)
    {
        List = observable.AsObservableChangeSet().AsObservableList();
        GroupKey = key;
    }

    public TGroupKey GroupKey { get; }
    public IObservableList<T> List { get; }
}