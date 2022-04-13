using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Utility.Service;
using UtilityInterface.NonGeneric;

namespace Utility.ViewModel;

public class FilterCollectionBaseViewModel<TR> : ReactiveObject
{
    protected readonly ReadOnlyObservableCollection<CheckViewModel> filterCollection;

    public FilterCollectionBaseViewModel(IObservable<IChangeSet<TR>> changeSet, Func<TR, string> keySelector, Settings settings)
    {
        changeSet
           .DistinctValues(keySelector)
           .Transform(a => new CheckViewModel(a, settings.DefaultValue))
           .Bind(out filterCollection)
           .Subscribe();
    }

    public virtual ICollection FilterCollection => filterCollection;
}

public class FilterCollectionKeyBaseViewModel<TR> : ReactiveObject where TR : IPredicate, IKey
{
    protected readonly ReadOnlyObservableCollection<CheckContentViewModel> filterCollection;

    public FilterCollectionKeyBaseViewModel(IObservable<IChangeSet<TR>> changeSet, Settings settings)
    {
        changeSet
            .Transform(a => new CheckContentViewModel(a, a.Key, settings.DefaultValue))
            .Bind(out filterCollection)
            .Subscribe();
    }

    public virtual ICollection FilterCollection => filterCollection;
}

public class FilterCollectionViewModel<T> : FilterCollectionBaseViewModel<T>
{
    private readonly ReactiveCommand<Dictionary<object, bool?>, Dictionary<string, bool>> command;

    public FilterCollectionViewModel(IObservable<IChangeSet<T>> changeSet, FilterDictionaryService<T> filter, Settings settings) :
        base(changeSet, filter.KeySelector, settings)
    {
        command = ReactiveCommand.Create<Dictionary<object, bool?>, Dictionary<string, bool>>(dictionary =>
        {
            var output = dictionary
                            .Where(a => a.Key != default && a.Value.HasValue)
                            .ToDictionary(a => (string)a.Key, a => a.Value!.Value);
            return output;
        });

        command.Subscribe(filter);
    }

    public ICommand Command => command;
}

public class FilterCollectionCommandViewModel<T, TR> : FilterCollectionKeyBaseViewModel<TR> where TR : IPredicate, IKey
{
    //private readonly ReadOnlyObservableCollection<CheckContentViewModel> filterCollection;
    private readonly ReactiveCommand<Dictionary<object, bool?>, Func<T, bool>> command;

    //private Dictionary<object, bool?>? dictionary;

    public FilterCollectionCommandViewModel(IObservable<IChangeSet<TR>> changeSet, FilterService<T> filterService, Settings settings) : base(changeSet, settings)
    {
        //changeSet.Transform(a =>
        //{
        //    foreach (var x in filterCollection.Select(a => a.Content).OfType<IAdd>())
        //    {
        //        x.Add(a);
        //    }
        //    return a;
        //}).Subscribe();

        command = ReactiveCommand.Create<Dictionary<object, bool?>, Func<T, bool>>(dictionary =>
        {
            //var output = from kvp in dictionary
            //             join item in filterCollection
            //             on kvp.Key.ToString() equals item.Header
            //             where kvp.Value == true
            //             select item;

            return a => filterCollection.All(o => !o.IsChecked || ((TR)o.Content).Invoke(a));
        });

        command.Subscribe(filterService);
        //command.Select(a => Unit.Default).Subscribe(replaySubject);
    }

    public ICommand Command => command;
}

public class FilterCollectionViewModel<T, TR> : FilterCollectionKeyBaseViewModel<TR> where TR : IPredicate, IKey
{
    public FilterCollectionViewModel(IObservable<IChangeSet<TR>> changeSet, Func<T, bool> filter, FilterService<T> filterService, Settings settings) : base(changeSet, settings)
    {
        Observable.Return(filter).Subscribe(filterService);
    }
}

public record Settings(bool DefaultValue = true);