using System;
using System.Windows;
using System.Windows.Input;

namespace UtilityWpf.Attached
{

    public enum State
    {
        None,
        Ticked, 
        Crossed,
        Refreshable

    }

    public partial class Ex : DependencyObject
    {
        public static readonly DependencyProperty SecurityIdProperty = DependencyProperty.RegisterAttached("SecurityId", typeof(object), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //SetSecurityId(d, e.NewValue);
        }

        public static string GetSecurityId(DependencyObject d)
        {
            return (string)d.GetValue(SecurityIdProperty);
        }

        public static void SetSecurityId(DependencyObject d, object value)
        {
            d.SetValue(SecurityIdProperty, value);
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached("Key", typeof(object), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        public static string GetKey(DependencyObject d)
        {
            return (string)d.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject d, object value)
        {
            d.SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsReadOnly(DependencyObject d)
        {
            return (bool)d.GetValue(IsReadOnlyProperty);
        }

        public static void SetIsReadOnly(DependencyObject d, object value)
        {
            d.SetValue(IsReadOnlyProperty, (bool)value);
        }


        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached("State", typeof(State), typeof(Ex), new PropertyMetadata(State.None, PropertyChanged));

        public static State GetState(DependencyObject d)
        {
            return (State)d.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject d, object value)
        {
            d.SetValue(StateProperty, (State)value);
        }
            
        
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached("IsPressed", typeof(bool), typeof(Ex), new PropertyMetadata(false, Property3Changed));

        private static void Property3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static bool GetIsPressed(DependencyObject d)
        {
            return (bool)d.GetValue(IsPressedProperty);
        }

        public static void SetIsPressed(DependencyObject d, object value)
        {
            d.SetValue(IsPressedProperty, (bool)value);
        }


        public static readonly DependencyProperty IsMouseOverProperty = DependencyProperty.RegisterAttached("IsMouseOver", typeof(bool), typeof(Ex), new PropertyMetadata(false, Property3Changed));

   
        public static bool GetIsMouseOver(DependencyObject d)
        {
            return (bool)d.GetValue(IsMouseOverProperty);
        }

        public static void SetIsMouseOver(DependencyObject d, object value)
        {
            d.SetValue(IsMouseOverProperty, (bool)value);
        }


        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject d, object value)
        {
            d.SetValue(CommandProperty, (ICommand)value);
        }


        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked", typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsChecked(DependencyObject d)
        {
            return (bool)d.GetValue(IsCheckedProperty);
        }

        public static void SetIsChecked(DependencyObject d, object value)
        {
            d.SetValue(IsCheckedProperty, (bool)value);
        }
    }
}