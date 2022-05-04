using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Common.Enum;
using UtilityWpf.Abstract;
using UtilityWpf.Attached;
using UtilityWpf.Utility;

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

    internal static class LayOutHelper
    {
        public static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
            {
                if (e.NewValue is Orientation orientation)
                {
                    var arrangement = (Arrangement)d.GetValue(ItemsControlEx.ArrangementProperty);
                    Changed(itemsControl, orientation, arrangement);
                }
            }
        }

        public static void ArrangementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
            {
                if (e.NewValue is Arrangement arrangement)
                {
                    var orientation = (Orientation)d.GetValue(ItemsControlEx.OrientationProperty);
                    Changed(itemsControl, orientation, arrangement);
                }
            }
        }

        public static void Changed(ItemsControl itemsControl, Orientation orientation, Arrangement arrangement)
        {
            switch (arrangement, orientation)
            {
                case (Arrangement.Stacked, Orientation.Vertical):
                    {
                        itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<StackPanel>(factory =>
                        {
                            factory.SetValue(StackPanel.OrientationProperty, orientation);
                        });
                        break;
                    }
                case (Arrangement.Wrapped, Orientation.Vertical):
                    {
                        itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<WrapPanel>(factory =>
                        {
                            factory.SetValue(WrapPanel.OrientationProperty, orientation);
                        });
                        break;
                    }
                case (Arrangement.Uniform, Orientation.Vertical):
                    {
                        itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<UniformGrid>(factory =>
                        {
                            factory.SetValue(UniformGrid.ColumnsProperty, 1);
                            factory.SetValue(UniformGrid.RowsProperty, itemsControl.Items.Count);
                        });
                        break;
                    }
                case (Arrangement.Stacked, Orientation.Horizontal):
                    {
                        itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<StackPanel>(factory =>
                        {
                            factory.SetValue(StackPanel.OrientationProperty, orientation);
                        });
                        break;
                    }
                case (Arrangement.Wrapped, Orientation.Horizontal):
                    {
                        itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<WrapPanel>(factory =>
                        {
                            factory.SetValue(WrapPanel.OrientationProperty, orientation);
                        });
                        break;
                    }
                case (Arrangement.Uniform, Orientation.Horizontal):
                    {
                        itemsControl.ItemsPanel = TemplateGenerator.CreateItemsPanelTemplate<UniformGrid>(factory =>
                        {
                            factory.SetValue(UniformGrid.RowsProperty, 1);
                            factory.SetValue(UniformGrid.ColumnsProperty, itemsControl.Items.Count);
                        });
                        break;
                    }
            }
        }
    }
}