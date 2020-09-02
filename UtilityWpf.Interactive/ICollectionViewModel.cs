using System.Collections.Generic;

namespace UtilityWpf.ViewModel
{
    public interface ICollectionViewModel<T>
    {
        ICollection<T> Items { get; }
    }
}