using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf.Controls
{
    public class MasterNotesControl : MasterBindableControl
    {
        static MasterNotesControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesControl), new FrameworkPropertyMetadata(typeof(MasterNotesControl)));
        }

        public override void OnApplyTemplate()
        {
            this.Content = new MasterNotesItemsControl
            {
                DisplayMemberPath = this.DisplayMemberPath,
                ItemsSource = this.ItemsSource
            };

            base.OnApplyTemplate();
        }
    }

    public class MasterNotesItemsControl : DragablzVerticalItemsControl
    {
        static MasterNotesItemsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesItemsControl), new FrameworkPropertyMetadata(typeof(MasterNotesItemsControl)));
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();
            if (element.ChildOfType<TextBox>() is not TextBox textBox)
                return;
            if (string.IsNullOrEmpty(DisplayMemberPath))
            {
                return;
            }
            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, myBinding);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
