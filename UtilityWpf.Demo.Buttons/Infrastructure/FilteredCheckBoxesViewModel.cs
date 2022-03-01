using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Service;
using Utility.ViewModel;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Buttons.Infrastructure
{
    internal class FilteredCheckBoxesViewModel
    {
        public FilteredCheckBoxesViewModel()
        {
            FilterDictionaryService<Profile> filter = new(a => a.Name);
            var changeSet = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();

            FilterCollectionViewModel = new(changeSet, filter, new(true));
            CollectionViewModel = new(changeSet, filter);
            CountViewModel = new(changeSet);
        }

        public FilterCollectionViewModel<Profile> FilterCollectionViewModel { get; }
        public CollectionViewModel<Profile> CollectionViewModel { get; }
        public CountViewModel CountViewModel { get; }
    }

    internal class FilteredCustomCheckBoxesViewModel
    {
        private readonly FilterService<Profile> filter = new();

        public FilteredCustomCheckBoxesViewModel()
        {
            var filters = new ProfileFilterCollectionObservable()
                            .ToObservableChangeSet();

            var profiles = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();
            Dictionary<Filter, IDisposable> dictionary = new();
            filters
                .OnItemAdded(filter =>
                {
                    if (filter is ObserverFilter<Profile> oFilter)
                        dictionary[filter] = profiles.Subscribe(oFilter);
                })
                .OnItemRemoved(filter =>
                {
                    if (dictionary.ContainsKey(filter))
                    {
                        dictionary[filter].Dispose();
                        dictionary.Remove(filter);
                    }
                })
                .Subscribe();

            FilterCollectionViewModel = new(filters, filter, new(false));
            CollectionViewModel = new(profiles, filter);
            CountViewModel = new(profiles);
            FilteredCountViewModel = new(profiles, filter);
        }

        public FilterCollectionCommandViewModel<Profile, Filter> FilterCollectionViewModel { get; }
        public CollectionViewModel<Profile> CollectionViewModel { get; }
        public CountViewModel CountViewModel { get; }
        public FilteredCountViewModel<Profile> FilteredCountViewModel { get; }
    }
}