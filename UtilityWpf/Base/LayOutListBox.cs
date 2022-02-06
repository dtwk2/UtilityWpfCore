using System;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Abstract;
using UtilityWpf.Utility;

namespace UtilityWpf.Base
{
    public class LayOutListBox : ListBox, IOrientation, IWrapping
    {
        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(typeof(LayOutListBox), new PropertyMetadata(LayOutHelper.Changed));


        public static readonly DependencyProperty IsWrappingProperty = DependencyProperty.Register("IsWrapping", typeof(bool), typeof(LayOutListBox), new PropertyMetadata(false));

        public bool IsWrapping
        {
            get => (bool)GetValue(IsWrappingProperty);
            set => SetValue(IsWrappingProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
    }

    public class LayOutItemsControl : ItemsControl, IOrientation
    {
        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(typeof(LayOutItemsControl), new PropertyMetadata(LayOutHelper.Changed));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }


    }

    internal static class LayOutHelper
    {
        public static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LayOutListBox dynamicItemsControl)
            {
                dynamicItemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate(factory =>
                    {
                        factory.SetValue(StackPanel.OrientationProperty, dynamicItemsControl.Orientation);
                    }, dynamicItemsControl.IsWrapping ? typeof(WrapPanel) : typeof(StackPanel));
            }
            else if (d is ItemsControl itemsControl && e.NewValue is Orientation orientation)
            {
                itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<StackPanel>(factory =>
                {
                    factory.SetValue(StackPanel.OrientationProperty, orientation);
                });
            }
        }
    }
}
