using System.Windows;

namespace UtilityWpf.Attached
{
    public partial class Ex : DependencyObject
    {
        public static readonly DependencyProperty SecurityIdProperty = DependencyProperty.RegisterAttached("SecurityId", typeof(object), typeof(Ex), new PropertyMetadata(null, asaas));

        private static void asaas(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached("Key", typeof(object), typeof(Ex), new PropertyMetadata(null, asaas));

        public static string GetKey(DependencyObject d)
        {
            return (string)d.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject d, object value)
        {
            d.SetValue(KeyProperty, value);
        }
    }
}