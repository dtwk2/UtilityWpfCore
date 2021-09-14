using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Dragablz;
using Utility.Common;

namespace UtilityWpf.Controls.Dragablz
{
    public class TicksControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(TicksControl), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandPathProperty = DependencyProperty.Register("CommandPath", typeof(string), typeof(TicksControl), new PropertyMetadata(null));

        static TicksControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TicksControl), new FrameworkPropertyMetadata(typeof(TicksControl)));
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }      
        
        public string CommandPath
        {
            get => (string)GetValue(CommandPathProperty);
            set => SetValue(CommandPathProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();


            CreateAndSetTextBinding();
            CreateAndSetCommandBinding();
            CreateAndSetIsCheckedBinding();

            base.PrepareContainerForItemOverride(element, item);

            void CreateAndSetTextBinding()
            {
                if (string.IsNullOrEmpty(DisplayMemberPath))
                    return;
                if (element.ChildOfType<TextBox>() is not TextBox textBox)
                    return;

                textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
                textBox.GotFocus += TextBox_GotFocus;

                var isReadOnly = item.GetType().GetProperty(DisplayMemberPath).IsReadOnly();
                var binding = new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                textBox.IsReadOnly = isReadOnly;
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            }

            void CreateAndSetIsCheckedBinding()
            {
                BindingOperations.SetBinding(element, Attached.Ex.IsCheckedProperty, CreateIsCheckedBinding());

                Binding CreateIsCheckedBinding()
                {
                    if (string.IsNullOrEmpty(IsCheckedPath))
                        throw new ArgumentNullException(nameof(IsCheckedPath));

                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(IsCheckedPath),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    return binding;
                }
            }
     

            void CreateAndSetCommandBinding()
            {
                if (element.ChildOfInterface<ICommandSource>() is not DependencyObject button || string.IsNullOrEmpty(CommandPath))
                    return;

                //if (element.ChildOfType<Button>() is not DependencyObject button || string.IsNullOrEmpty(CommandPath))
                //    return;
                BindingOperations.SetBinding(button, ButtonBase.CommandProperty, CreateCommandBinding(item));

                Binding? CreateCommandBinding(object item)
                {
                    if (string.IsNullOrEmpty(CommandPath))
                        return null;

                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(CommandPath),
                        Mode = BindingMode.OneTime
                    };
                    return binding;
                }
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
