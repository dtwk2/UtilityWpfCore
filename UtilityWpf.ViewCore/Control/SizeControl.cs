using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class SizeControl : Controlx
    {
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(Control), new PropertyMetadata(25, Changed, CoerceSize));

        public static readonly DependencyProperty TotalSizeProperty = DependencyProperty.Register("TotalSize", typeof(int), typeof(Control), new PropertyMetadata(100, Changed, CoerceTotalSize));

        private static object CoerceSize(DependencyObject d, object baseValue)
        {
            return Math.Max(1, Math.Min((int)baseValue, (d as SizeControl).TotalSize));
        }

        private static object CoerceTotalSize(DependencyObject d, object baseValue)
        {
            return Math.Max((int)baseValue, 1);
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public int TotalSize
        {
            get { return (int)GetValue(TotalSizeProperty); }
            set { SetValue(TotalSizeProperty, value); }
        }

        public SizeControl()
        {
            Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/SizeControlStyle.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Style = resourceDictionary["SizeControlStyle"] as Style;

            base.GetChanges(nameof(Size)).Select(_ => (int)_).Subscribe(RaiseSelectedSizeEvent);
        }

        public static readonly RoutedEvent SelectedSizeChangedEvent = EventManager.RegisterRoutedEvent("SelectedSizeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SizeControl));

        public event RoutedEventHandler SelectedSizeChanged
        {
            add { AddHandler(SelectedSizeChangedEvent, value); }
            remove { RemoveHandler(SelectedSizeChangedEvent, value); }
        }

        private void RaiseSelectedSizeEvent(int Size)
        {
            SelectedSizeChangedRoutedEventArgs newEventArgs = new SelectedSizeChangedRoutedEventArgs(SizeControl.SelectedSizeChangedEvent) { Size = Size };
            RaiseEvent(newEventArgs);
        }

        public class SelectedSizeChangedRoutedEventArgs : RoutedEventArgs
        {
            public int Size { get; set; }

            public SelectedSizeChangedRoutedEventArgs(RoutedEvent @event) : base(@event)
            {
            }
        }
    }
}