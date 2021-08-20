using System.Windows.Controls;

namespace UtilityWpf.Abstract
{
    public interface ISelectionChanged
    {
        event SelectionChangedEventHandler SelectionChanged;
    }
}