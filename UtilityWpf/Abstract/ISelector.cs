using System.Collections;
using System.Windows.Controls;

namespace UtilityWpf.Abstract
{
    public interface ISelector
    {
        object SelectedItem { get; }
        int SelectedIndex { get; }

        event SelectionChangedEventHandler SelectionChanged;

        IEnumerable ItemsSource { get; }
    }   
}