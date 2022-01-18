using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using Utility.Common;

namespace Utility.ViewModel;

public static class GroupHelper
{
    public static IObservable<IChangeSet<GroupViewModel<T, TKey, TGroupKey>, TGroupKey>> Convert<T, TKey, TGroupKey>(this IObservable<IGroupChangeSet<T, TKey, TGroupKey>> groups, Func<IGroup<T, TKey, TGroupKey>, GroupViewModel<T, TKey, TGroupKey>> createFunc)
    {
        return groups
             .Transform(createFunc)
             .ObserveOn(RxApp.MainThreadScheduler);
    }

    public static IObservable<IChangeSet<GroupViewModel<T, TKey, TGroupKey>, TGroupKey>> Convert<TGroup, T, TKey, TGroupKey>(this IObservable<IGroupChangeSet<TGroup, TKey, TGroupKey>> groups)
        where TGroup : IGroupable<T>
        where T : class
    {
        return groups
            .Transform(t => new GroupViewModel<T, TKey, TGroupKey>(new Group<T, TKey, TGroupKey>(t.Key, t.Cache.Cast(a => a.Value))))
            .ObserveOn(RxApp.MainThreadScheduler);
    }

    public static IObservable<IChangeSet<GroupViewModel<T, TGroupKey>>> Convert<TGroup, T, TGroupKey>(this IObservable<IChangeSet<IGroup<TGroup, TGroupKey>>> groups)
        where TGroup : IGroupable<T>
        where T : class
    {
        return groups
            .Transform(t => new GroupViewModel<T, TGroupKey>(new Group<T, TGroupKey>(t.GroupKey, t.List.Items.Select(a => a.Value))))
            .ObserveOn(RxApp.MainThreadScheduler);
    }
}