using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace UtilityWpf.Adorners
{
    public class Error
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(Error), new FrameworkPropertyMetadata(string.Empty, OnChanged));

        public static string GetText(UIElement adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            return (string)adornedElement.GetValue(TextProperty);
        }

        public static void SetText(UIElement adornedElement, string placeholderText)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            //offset = -adornedElement.Height;
            adornedElement.SetValue(TextProperty, placeholderText);
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (((string)e.OldValue).Equals((string)e.NewValue, StringComparison.Ordinal))
                return;
            if (sender is not FrameworkElement adornedElement)
                return;
            if (adornedElement.IsLoaded)
                AddAdorner(adornedElement);
            else
                adornedElement.Loaded += AdornedElement_Loaded;

            void AddAdorner(FrameworkElement adornedElement)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                Text.InvalidateAdorners(adornerLayer, adornedElement);
                adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Right, Place.Outside, Brushes.Red));
            }
            void AdornedElement_Loaded(object sender, RoutedEventArgs e)
            {
                FrameworkElement adornedElement = (FrameworkElement)sender;
                //adornedElement.Loaded -= AdornedElement_Loaded;
                if (string.IsNullOrEmpty(GetText(adornedElement)) == false)
                    AddAdorner(adornedElement);
                else
                {
                }
            }
        }
    }
}
