using System.Collections;
using UtilityWpf.Events;

namespace UtilityWpf.Abstract
{
    public interface ICheckedSelector
    {
        IEnumerable CheckedItems { get; }
        IEnumerable UnCheckedItems { get; }

        event CheckedChangedEventHandler CheckedChanged;
    }
}