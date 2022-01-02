using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Common;
using Utility.Common.Helper;
using Utility.Common.Model;

namespace Utility.ViewModel;

public static class GroupCollectionBuilder
{
    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<T, TKey, TGroupKey> ToGroupOnViewModel<T, TKey, TGroupKey>(this IObservable<IChangeSet<T, TKey>> changeSet, Expression<Func<T, TGroupKey>> func)
        where T : INotifyPropertyChanged
    {
        return new(changeSet.GroupOnProperty(func));
    }

    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<TGroup, TR, TKey, string> ToGroupOnViewModel<TGroup, TR, TKey>(this IObservable<IChangeSet<TGroup, TKey>> changeSet)
        where TGroup : Groupable<TR>
        where TR : class
    {
        return new(changeSet.GroupOnProperty(a => a.GroupProperty));
    }

    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<Groupable<TR>, TR, TKey, string> ToGroupOnViewModel<TR, TKey>(this IObservable<IChangeSet<Groupable<TR>, TKey>> changeSet)
        where TR : class
    {
        return changeSet.ToGroupOnViewModel<Groupable<TR>, TR, TKey>();
    }

    /// <summary>
    /// For when the group property changes and have no key
    /// </summary>
    public static GroupCollection2ViewModel<Groupable<TR>, TR, string> ToGroupOnViewModel<TR>(this IObservable<IChangeSet<Groupable<TR>>> changeSet)
        where TR : class
    {
        return new(changeSet.GroupOnProperty(a => a.GroupProperty));
    }

    /// <summary>
    /// For when the group property changes
    /// </summary>
    public static GroupCollectionViewModel<Groupable, object, TKey, string> ToGroupOnViewModel<TKey>(this IObservable<IChangeSet<Groupable, TKey>> changeSet)
    {
        return new(changeSet.GroupOnProperty(a => a.GroupProperty));
    }

    /// <summary>
    /// For when the group property never changes
    /// </summary>
    public static GroupCollectionViewModel<T, TKey, TGroupKey> ToGroupViewModel<T, TKey, TGroupKey>(this IObservable<IChangeSet<T, TKey>> changeSet, Func<T, TGroupKey> func)
    {
        return new(changeSet.Group(func));
    }
}

public class GroupCollectionViewModel<TGroupable, T, TKey, TGroupKey> : GroupCollectionViewModel
    where TGroupable : IGroupable<T>
    where T : class
{
    private readonly ReadOnlyObservableCollection<GroupViewModel<T, TKey, TGroupKey>> collection;

    public GroupCollectionViewModel(IObservable<IGroupChangeSet<TGroupable, TKey, TGroupKey>> changeSet)
    {
        collection = GroupHelper.Convert<TGroupable, T, TKey, TGroupKey>(changeSet).ToCollection(out var disposable);
        CompositeDisposable.Add(disposable);
    }

    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Collection => collection;
}

public class GroupCollectionViewModel<T, TKey, TGroupKey> : GroupCollectionViewModel
{
    public GroupCollectionViewModel(IObservable<IGroupChangeSet<T, TKey, TGroupKey>> changeSet)
    {
        Collection = GroupHelper.Convert(changeSet, Create).ToCollection(CompositeDisposable);
    }

    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Collection { get; }

    public virtual GroupViewModel<T, TKey, TGroupKey> Create(IGroup<T, TKey, TGroupKey> group)
    {
        return new GroupViewModel<T, TKey, TGroupKey>(group);
    }
}

public class GroupCollection2ViewModel<TGroupable, T, TGroupKey> : GroupCollectionViewModel
    where TGroupable : IGroupable<T>
    where T : class
{
    public GroupCollection2ViewModel(IObservable<IChangeSet<IGroup<TGroupable, TGroupKey>>> changeSet)
    {
        Collection = GroupHelper.Convert<TGroupable, T, TGroupKey>(changeSet).ToCollection(CompositeDisposable);
    }

    public override IReadOnlyCollection<ClassProperty> Properties => typeof(T).GetProperties().Select(a => new ClassProperty(a.Name, typeof(T).Name)).ToArray();

    public override ICollection Collection { get; }

    public virtual GroupViewModel<T, TGroupKey> Create(IGroup<T, TGroupKey> group)
    {
        return new GroupViewModel<T, TGroupKey>(group);
    }
}

public abstract class GroupCollectionViewModel : BaseDisposable
{
    public virtual IReadOnlyCollection<ClassProperty> Properties { get; }

    public virtual ICollection Collection { get; }
}