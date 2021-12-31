using DynamicData;
using System.Linq;
using System.Reactive.Linq;
using Utility.ViewModel;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Buttons.Infrastructure
{

    internal class FilteredCheckBoxesViewModel
    {
        public FilteredCheckBoxesViewModel()
        {
            Filter<Profile> filter = new(a => a.Name);
            var changeSet = new ProfileCollectionObservable(10, 5)
                            .Take(20)
                            .ToObservableChangeSet();

            FilterCollectionViewModel = new(changeSet, filter);
            CollectionViewModel = new(changeSet, filter);
            CountViewModel = new(changeSet);
        }

        public FilterCollectionViewModel<Profile> FilterCollectionViewModel { get; }
        public CollectionViewModel<Profile> CollectionViewModel { get; }
        public CountViewModel CountViewModel { get; }
    }
}