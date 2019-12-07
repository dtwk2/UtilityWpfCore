using System;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class HeaderBodyControl : HeaderedContentControl
    {
        static HeaderBodyControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderBodyControl), new FrameworkPropertyMetadata(typeof(HeaderBodyControl)));
        }

        public HeaderBodyControl()
        {
            Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/HeaderBodyControl.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Style = resourceDictionary["HeaderBodyControlStyle"] as Style;
        }

        public FrameworkElement Body
        {
            get { return (FrameworkElement)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Body.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(FrameworkElement), typeof(HeaderBodyControl), new UIPropertyMetadata(null, Change));

        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}