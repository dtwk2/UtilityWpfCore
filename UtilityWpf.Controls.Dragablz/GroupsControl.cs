using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf.Controls.Dragablz
{
    public class GroupsControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty IsReadOnlyPathProperty = DependencyProperty.Register("IsReadOnlyPath", typeof(string), typeof(GroupsControl), new PropertyMetadata(null));

        static GroupsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupsControl), new FrameworkPropertyMetadata(typeof(GroupsControl)));
        }

        public string IsReadOnlyPath
        {
            get => (string)GetValue(IsReadOnlyPathProperty);
            set => SetValue(IsReadOnlyPathProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is Control control)
            {
                SetBinding(control, item);
                SetIsReadOnlyBinding(control, item);
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        private void SetBinding(Control element, object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;
            _ = element.ApplyTemplate();
            if (element.ChildOfType<TextBlock>() is not TextBlock textBlock)
                return;

            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, myBinding);
        }

        private void SetIsReadOnlyBinding(Control element, object item)
        {
            if (string.IsNullOrEmpty(IsReadOnlyPath))
                return;

            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(IsReadOnlyPath),
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(element, Attached.Ex.IsReadOnlyProperty, myBinding);
        }
    }
}