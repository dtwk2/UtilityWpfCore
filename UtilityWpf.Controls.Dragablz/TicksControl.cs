using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Dragablz;

namespace UtilityWpf.Controls.Dragablz
{
    public class TicksControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(TicksControl), new PropertyMetadata(null));

        static TicksControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TicksControl), new FrameworkPropertyMetadata(typeof(TicksControl)));
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();


            CreateAndSetTextBinding(item);
    
            BindingOperations.SetBinding(element, Attached.Ex.IsCheckedProperty, CreateIsCheckedBinding(item));

 
            base.PrepareContainerForItemOverride(element, item);

            void CreateAndSetTextBinding(object item)
            {
                if (string.IsNullOrEmpty(DisplayMemberPath))
                    return;
                if (element.ChildOfType<TextBox>() is not TextBox textBox)
                    return;

                textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
                textBox.GotFocus += TextBox_GotFocus;

                var binding= new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };

                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            }

            Binding CreateIsCheckedBinding(object item)
            {
                if (string.IsNullOrEmpty(IsCheckedPath))
                    throw new ArgumentNullException(nameof(IsCheckedPath));

                Binding myBinding = new Binding
                {
                    Source = item,
                    Path = new PropertyPath(IsCheckedPath),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                return myBinding;
            }

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void TextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }
    }
}
