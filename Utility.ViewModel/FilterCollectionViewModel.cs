using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Utility.Service;
using UtilityInterface.NonGeneric;
using UtilityWpf;

namespace Utility.ViewModel;

public class FilterCollectionViewModel<T> : ReactiveObject
{
    public record Settings(bool DefaultValue = true);

    private readonly ReadOnlyObservableCollection<CheckViewModel> filterCollection;
    private readonly ReactiveCommand<Dictionary<object, bool?>, Dictionary<string, bool>> command;

    public FilterCollectionViewModel(IObservable<IChangeSet<T>> changeSet, FilterDictionaryService<T> filter, Settings settings)
    {
        changeSet
            .DistinctValues(filter.KeySelector)
            .Transform(a => new CheckViewModel(a, settings.DefaultValue))
            .Bind(out filterCollection)
            .Subscribe();

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

    public ICollection FilterCollection => filterCollection;
}

public class FilterCollectionCommandViewModel<T, TR> : FilterCollectionBaseViewModel<TR> where TR : IPredicate, IKey
{
    private readonly ReadOnlyObservableCollection<CheckContentViewModel> filterCollection;
    private readonly ReactiveCommand<Dictionary<object, bool?>, Func<T, bool>> command;
    //private Dictionary<object, bool?>? dictionary;

    public FilterCollectionCommandViewModel(IObservable<IChangeSet<TR>> changeSet, FilterService<T> filter, Settings settings) : base(changeSet, settings)
    {
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
        command.Select(a => Unit.Default).Subscribe(replaySubject);
    }

    public ICommand Command => command;
}

public class FilterCollectionViewModel<T, TR> : FilterCollectionBaseViewModel<TR> where TR : IPredicate, IKey
{
    public FilterCollectionViewModel(IObservable<IChangeSet<TR>> changeSet, Func<T, bool> filter, FilterService<T> filterService, Settings settings) : base(changeSet, settings)
    {
        Observable.Return(filter).Subscribe(filterService);
    }
}

public class FilterCollectionBaseViewModel<TR> : ReactiveObject where TR : IPredicate, IKey
{
    public record Settings(bool DefaultValue = true);

    private readonly ReadOnlyObservableCollection<CheckContentViewModel> filterCollection;

    public FilterCollectionBaseViewModel(IObservable<IChangeSet<TR>> changeSet, Settings settings)
    {
        changeSet
            .Transform(a => new CheckContentViewModel(a, a.Key, settings.DefaultValue))
            .Bind(out filterCollection)
            .Subscribe();
    }

    public ICollection FilterCollection => filterCollection;
}