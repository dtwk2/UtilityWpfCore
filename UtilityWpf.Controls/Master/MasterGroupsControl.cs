using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace UtilityWpf.Controls
{
    public class MasterGroupsControl : MasterBindableControl
    {
        static MasterGroupsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsControl)));
        }

        public override void OnApplyTemplate()
        {
            this.Content = new MasterGroupsItemsControl
            {
                DisplayMemberPath = this.DisplayMemberPath,
                ItemsSource = this.ItemsSource
            };   
            base.OnApplyTemplate();
        }
    }

    public class MasterGroupsItemsControl : DragablzVerticalItemsControl
    {
        static MasterGroupsItemsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsItemsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsItemsControl)));
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();
            if (element.ChildOfType<TextBlock>() is not TextBlock textBox)
                return;
            if (string.IsNullOrEmpty(DisplayMemberPath))
            {
                return;
            }
            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(textBox, TextBlock.TextProperty, myBinding);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
