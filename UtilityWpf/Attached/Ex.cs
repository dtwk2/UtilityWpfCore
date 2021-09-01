using System.Windows;

namespace UtilityWpf.Attached
{
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
            d.SetValue(KeyProperty, (bool)value);
        }


        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked", typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsChecked(DependencyObject d)
        {
            return (bool)d.GetValue(IsCheckedProperty);
        }

        public static void SetIsChecked(DependencyObject d, object value)
        {
            d.SetValue(KeyProperty, (bool)value);
        }
    }
}