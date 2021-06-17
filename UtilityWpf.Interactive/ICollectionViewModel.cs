using System.Collections.Generic;

namespace UtilityWpf.Model
{
    public interface ICollectionViewModel<T>
    {
        ICollection<T> Items { get; }
    }
}