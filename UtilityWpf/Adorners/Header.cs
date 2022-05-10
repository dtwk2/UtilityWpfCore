namespace UtilityWpf.Adorners
{
    using System;
using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class Header
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(Header), new FrameworkPropertyMetadata(string.Empty, OnChanged));

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
                //foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                //{
                //    if (adorner is Text && Header.GetText(adornedElement) != null)
                //        adornerLayer.Remove(adorner);
                //}
                foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                {
                    if (adorner is Text text
                        && (Dock?)adorner.GetValue(Text.PositionProperty) == Dock.Top
                        && (Place?)adorner.GetValue(Text.PlaceProperty) == Place.Outside)
                    {
                        adornerLayer.Remove(adorner);
                    }
                    var text1 = adorner.GetValue(Text.TextProperty);
                    if (string.IsNullOrEmpty(text1.ToString()))
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
                //Text.InvalidateAdorners(adornerLayer, adornedElement);
                adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Top, Place.Outside));
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

    public class LeftHeader
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
       "Text", typeof(string), typeof(LeftHeader), new FrameworkPropertyMetadata(string.Empty, OnChanged));

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
                Text.SetText(adornedElement, (string)e.NewValue);
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                //foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                //{
                //    if (adorner is Text && LeftHeader.GetText(adornedElement) != null)
                //        adornerLayer.Remove(adorner);
                //}
                //Text.InvalidateAdorners(adornerLayer, adornedElement);
                //if ((adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>()).SingleOrDefault() is Adorner adorner
                //    && (Dock)adorner.GetValue(Text.PositionProperty)== Dock.Left 
                //    && (Place)adorner.GetValue(Text.PlaceProperty) == Place.Outside)
                //{
                //    adorner.SetValue(Text.TextProperty, e.NewValue);
                //}
                //else
                foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                {
                    if (adorner is Text text
                        && (Dock?)adorner.GetValue(Text.PositionProperty) == Dock.Left
                        && (Place?)adorner.GetValue(Text.PlaceProperty) == Place.Outside)
                    {
                        adornerLayer.Remove(adorner);
                    }
                    var text1 = adorner.GetValue(Text.TextProperty);
                    if (string.IsNullOrEmpty(text1.ToString()))
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
                adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Left, Place.Outside));
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