using DynamicData;
using DynamicData.PLinq;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Common;

namespace Utility.ViewModel;

public class CollectionViewModel : ReactiveObject
{
    public virtual ICollection Collection { get; }
}

public class CollectionViewModel<T> : CollectionViewModel
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

    public override ICollection Collection => collection;
}

public class CollectionViewModel<T, TKey> : CollectionViewModel
{
    private readonly ReadOnlyObservableCollection<T> collection;

    public CollectionViewModel(IObservable<IChangeSet<T, TKey>> changeSet, Filter<T> filter)
    {
        changeSet
            .Filter(filter)
            .Select(a => a)
            .Bind(out collection)
            .Subscribe();
    }

    public override ICollection Collection => collection;
}

public class CollectionViewModel<T, TKey, TGroupKey> : CollectionViewModel
{
    public CollectionViewModel(IObservable<IChangeSet<T, TKey>> changeSet, Filter<T> filter, Func<T, TGroupKey> group)
    {
        Collection = changeSet
            .Filter(filter)
            .ToGroupViewModel(group)
            .Collection;
    }

    public override ICollection Collection { get; }
}

public class CollectionGroupViewModel<T, TKey> : CollectionViewModel, IObserver<ClassProperty> where T : class
{
    private ReplaySubject<ClassProperty> subject = new(1);
    private readonly GroupCollectionViewModel<Groupable<T>, T, TKey, string> viewModel;

    public CollectionGroupViewModel(IObservable<IChangeSet<T, TKey>> changeSet, Filter<T> filter, string propertyName)
    {
        var type = typeof(T).Name;

        viewModel = changeSet
            .Filter(filter)
            .Transform(a => new Groupable<T>(a, new ClassProperty(propertyName, type), subject))
            .ToGroupOnViewModel();

        //    var aa = GroupHelper.Convert<Groupable<T>, T, TKey, string>(
        //changeSet
        //.Filter(filter)
        //.Transform(a => new Groupable<T>(a, new ClassProperty(propertyName, type), subject))
        //.GroupOnProperty(a => a.GroupProperty)
        //);
    }

    public override ICollection Collection => viewModel.Collection;

    public IReadOnlyCollection<ClassProperty> Properties => viewModel.Properties;

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ClassProperty value)
    {
        subject.OnNext(value);
    }
}

public class CollectionGroupViewModel<T> : CollectionViewModel, IObserver<ClassProperty> where T : class
{
    private ReplaySubject<ClassProperty> subject = new(1);
    private readonly GroupCollection2ViewModel<Groupable<T>, T, string> viewModel;

    public CollectionGroupViewModel(IObservable<IChangeSet<T>> changeSet, Filter<T> filter, string propertyName)
    {
        var type = typeof(T).Name;

        viewModel = changeSet
            .Filter(filter)
            .Transform(a => new Groupable<T>(a, new ClassProperty(propertyName, type), subject))
            .ToGroupOnViewModel();
    }

    public override ICollection Collection => viewModel.Collection;

    public IReadOnlyCollection<ClassProperty> Properties => viewModel.Properties;

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ClassProperty value)
    {
        subject.OnNext(value);
    }
}