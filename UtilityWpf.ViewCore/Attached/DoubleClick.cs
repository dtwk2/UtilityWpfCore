using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.View
{
    public class MouseDoubleClick : DependencyObject
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDoubleClick), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseDoubleClick), new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static object GetCommand(DependencyObject target)
        {
            return target.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            if (command.CanExecute(null))
            {
                command.Execute(commandParameter);
                e.Handled = true;
            }
        }

        //public class ControlDoubleClick : DependencyObject
        //{
        //    public static readonly DependencyProperty CommandProperty =
        //        DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ControlDoubleClick), new PropertyMetadata(OnChangedCommand));

        //    public static readonly DependencyProperty ValueProperty =
        //DependencyProperty.RegisterAttached("Value", typeof(bool), typeof(ControlDoubleClick), new PropertyMetadata(OnValueCommand));

        //    private static void OnValueCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //    }

        //    public static ICommand GetCommand(Control target)
        //    {
        //        return (ICommand)target.GetValue(CommandProperty);
        //    }

        //    public static void SetCommand(Control target, ICommand value)
        //    {
        //        target.SetValue(CommandProperty, value);
        //    }

        //    private static void OnChangedCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //    {
        //        Control control = d as Control;
        //        control.PreviewMouseDoubleClick += new MouseButtonEventHandler(Element_PreviewMouseDoubleClick);

        //    }

        //    private static void Element_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        //    {
        //        Control control = sender as Control;
        //        ICommand command = GetCommand(control);

        //        if (command.CanExecute(null))
        //        {
        //            command.Execute(null);
        //            e.Handled = true;
        //            control.SetValue(ValueProperty, true);
        //        }
        //    }

        //}
    }
}