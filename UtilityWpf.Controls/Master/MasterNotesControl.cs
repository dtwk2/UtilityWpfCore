using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Dragablz;

namespace UtilityWpf.Controls
{
    public class MasterNotesControl : MasterBindableControl
    {
        static MasterNotesControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesControl), new FrameworkPropertyMetadata(typeof(MasterNotesControl)));
        }

        public MasterNotesControl()
        {
            Orientation = Orientation.Vertical;
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

            textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
            textBox.GotFocus += TextBox_GotFocus;
            base.PrepareContainerForItemOverride(element, item);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void TextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if((e.OriginalSource as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }  
    }
}
