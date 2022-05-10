namespace UtilityWpf.Adorners
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class Footer
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
       "Text", typeof(string), typeof(Footer), new FrameworkPropertyMetadata(string.Empty, OnChanged));

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
                Text.InvalidateAdorners(adornerLayer, adornedElement);
                foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                {
                    if (adorner is Text text
                        && (Dock?)adorner.GetValue(Text.PositionProperty) == Dock.Bottom
                        && (Place?)adorner.GetValue(Text.PlaceProperty) == Place.Inside)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
                adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Bottom, Place.Inside));

                //foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                //    {
                //        if (adorner is Text && Footer.GetText(adornedElement) != null)
                //            adornerLayer.Remove(adorner);
                //    }

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