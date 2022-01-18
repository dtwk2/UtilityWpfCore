using DynamicData;
using System.Linq;
using System.Reactive.Linq;
using Utility.Service;
using Utility.ViewModel;
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

    internal class FilteredCheckBoxes2ViewModel
    {
        public FilteredCheckBoxes2ViewModel()
        {

            var observable = new ProfileFilterCollectionObservable().ToObservableChangeSet();

            FilterService<Profile> filter = new();
            var changeSet = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();

            FilterCollectionViewModel = new(observable, filter, new(false));
            CollectionViewModel = new(changeSet, filter);
            CountViewModel = new(changeSet);
        }

        public FilterCollectionViewModel<Profile, ProfileFilter> FilterCollectionViewModel { get; }
        public CollectionViewModel<Profile> CollectionViewModel { get; }
        public CountViewModel CountViewModel { get; }
    }
}