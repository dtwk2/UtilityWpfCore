namespace Utility.WPF.Adorners
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class Header
    {
        static List<Guid> guids = new();

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
            Text.OnChanged2(sender, (string)e.NewValue, guids, (f, t) => new Text(f, t, Dock.Top, Place.Outside));
        }
    }

    public class LeftHeader
    {
        static List<Guid> guids = new();
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
            Text.OnChanged2(sender, (string)e.NewValue, guids, (f, t) => new Text(f, t, Dock.Left, Place.Outside));

            //if (sender is not FrameworkElement adornedElement)
            //    throw new Exception("s42df234 dfg f,l,lgd");
            //if (adornedElement.IsLoaded)
            //    AddAdorner(adornedElement, e);
            //else
            //    adornedElement.Loaded += (s, a) => AddAdorner((FrameworkElement)sender, e);

            //void AddAdorner(FrameworkElement adornedElement, DependencyPropertyChangedEventArgs e)
            //{
            //    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);

            //    bool flag = false;
            //    foreach (var adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
            //    {
            //        if (adorner is Text { Guid: var guid } text)
            //        {
            //            if (guids.Contains(guid) == false)
            //                return;
            //            flag = true;
            //            text.text = (string)e.NewValue;
            //        }
            //    }

            //    if (flag == false)
            //    {
            //        var guid = Guid.NewGuid();
            //        guids.Add(guid);
            //        //adornerLayer.Add(new Text(adornedElement, (string)e.NewValue, Dock.Left, Place.Outside) { Guid = guid });
            //        var text = factory.Invoke(adornedElement);
            //        text.Guid = guid;
            //        text.text = (string)e.NewValue;
            //        adornerLayer.Add(text);
            //    }
            //    else
            //        Text.InvalidateTextAdorners(adornerLayer, adornedElement);
            //}
        }
    }
}