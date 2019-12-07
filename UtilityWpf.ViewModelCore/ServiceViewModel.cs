using DynamicData;
using DynamicData.Binding;
using System;
using System.Collections.ObjectModel;
using UtilityInterface.Generic;

namespace UtilityWpf.ViewModel
{
    public class ServiceViewModel<T>
    {
        public ObservableCollection<T> Items { get; }

        public ServiceViewModel(IService<T> service)
        {
            service.Resource.Subscribe(_ =>
            {
                Items.Add(_);
            });
        }
    }

    public class ServiceViewModel<T, R> where R : IComparable
    {
        public ReadOnlyObservableCollection<T> Items => _items;
        private readonly ReadOnlyObservableCollection<T> _items;

        public ServiceViewModel(UtilityInterface.Generic.IService<T> service, Func<T, R> sort, bool byDescending = true)
        {
            SortExpressionComparer<T> comparer = (byDescending) ?
               SortExpressionComparer<T>.Descending((t) => sort(t)) :
               SortExpressionComparer<T>.Ascending((t) => sort(t));

            service
                 .Resource
                 .ToObservableChangeSet()
                 .Sort(comparer)

                 .Bind(out _items)
                 .DisposeMany();
        }
    }
}