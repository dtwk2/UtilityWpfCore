using System.Collections;

namespace UtilityWpf.Abstract
{
    public interface IItemsSource
    {
        IEnumerable ItemsSource { get; set; }
    }
}