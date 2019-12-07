using DynamicData.Binding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UtilityWpf.ViewModel

{
    public class SortController<T> : AbstractNotifyPropertyChanged
    {
        private readonly IList<SortContainer<T>> _sortItems;

        private SortContainer<T> _selectedItem;

        public SortController(IEnumerable<SortContainer<T>> obs)
        {
            _sortItems = new ObservableCollection<SortContainer<T>>(obs);
            SelectedItem = _sortItems[2];
        }

        public SortContainer<T> SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(ref _selectedItem, value);
        }

        public IEnumerable<SortContainer<T>> SortItems => _sortItems;
    }
}