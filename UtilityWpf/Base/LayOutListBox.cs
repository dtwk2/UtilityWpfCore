using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Attached;
using UtilityWpf.Abstract;

namespace UtilityWpf.Base
{
    public class LayOutListBox : ListBox, IOrientation //, IWrapping
    {
        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(typeof(LayOutListBox), new PropertyMetadata(LayOutHelper.OrientationChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
    }

    public class LayOutItemsControl : ItemsControl, IOrientation
    {
        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(typeof(LayOutItemsControl), new PropertyMetadata(LayOutHelper.OrientationChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
    }

   
}