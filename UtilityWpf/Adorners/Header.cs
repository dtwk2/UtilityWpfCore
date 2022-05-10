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
            if (sender is not FrameworkElement adornedElement)
                throw new Exception("s42df456 dfg f,l,lgd");
            if (adornedElement.IsLoaded)
                AddAdorner(adornedElement, e);
            else
                adornedElement.Loaded += (s, e) => AddAdorner((FrameworkElement)sender, default);

            static void AddAdorner(FrameworkElement adornedElement, DependencyPropertyChangedEventArgs e)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);

                bool flag = false;
                foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                {
                    if (adorner is Text text)
                    {
                        flag = true;
                        text.text = (string)e.NewValue;
                    }
                }

                if (flag == false)
                    adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Top, Place.Outside));
                else
                    Text.InvalidateTextAdorners(adornerLayer, adornedElement);
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
            if (sender is not FrameworkElement adornedElement)
                throw new Exception("s42df234 dfg f,l,lgd");
            if (adornedElement.IsLoaded)
                AddAdorner(adornedElement, e);
            else
                adornedElement.Loaded += (s, e) => AddAdorner((FrameworkElement)sender, default);

            static void AddAdorner(FrameworkElement adornedElement, DependencyPropertyChangedEventArgs e)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);

                bool flag = false;
                foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
                {
                    if (adorner is Text text)
                    {
                        flag = true;
                        text.text = (string)e.NewValue;
                    }
                }

                if (flag == false)
                    adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Left, Place.Outside));
                else
                    Text.InvalidateTextAdorners(adornerLayer, adornedElement);
            }
        }
    }
}