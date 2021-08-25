using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Abstract
{
    public interface ISelector
    {
        object SelectedItem { get; }
        int SelectedIndex { get; }

        event SelectionChangedEventHandler SelectionChanged;
    }



}