using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace UtilityWpf.Adorners
{
    public class Type
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached(
            "Type", typeof(System.Type), typeof(Type), new FrameworkPropertyMetadata(null, OnChanged));

        public static System.Type? GetType(UIElement adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            return (System.Type?)adornedElement.GetValue(TypeProperty);
        }

        public static void SetType(UIElement adornedElement, System.Type placeholderType)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            //offset = -adornedElement.Height;
            adornedElement.SetValue(TypeProperty, placeholderType);
        }

        public static readonly DependencyProperty UseDataContextProperty = DependencyProperty.RegisterAttached(
            "UseDataContext", typeof(bool), typeof(System.Type), new FrameworkPropertyMetadata(false, OnChanged));

        public static bool GetUseDataContext(UIElement adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            return (bool)adornedElement.GetValue(UseDataContextProperty);
        }

        public static void SetUseDataContext(UIElement adornedElement, bool value)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            adornedElement.SetValue(UseDataContextProperty, value);
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
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
                adornerLayer.Add(new Text(adornedElement, Type(adornedElement)?.ToString(), Dock.Bottom, Place.Inside));
            }
            void AdornedElement_Loaded(object sender, RoutedEventArgs e)
            {
                FrameworkElement adornedElement = (FrameworkElement)sender;
                AddAdorner(adornedElement);
            }

            System.Type? Type(FrameworkElement adornedElement)
            {
                System.Type? type = GetType(adornedElement);
                System.Type? func() => GetUseDataContext(adornedElement) ? adornedElement.DataContext?.GetType() : adornedElement.GetType();
                return type ?? func();
            }
        }
    }
}