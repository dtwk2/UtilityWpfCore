﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Utility.WPF.Adorners;

public class Type
{
    //public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached(
    //    "Type", typeof(System.Type), typeof(Type), new FrameworkPropertyMetadata(null, OnChanged));

    //public static System.Type? GetType(UIElement adornedElement)
    //{
    //    if (adornedElement == null)
    //        throw new ArgumentNullException("adornedElement");
    //    return (System.Type?)adornedElement.GetValue(TypeProperty);
    //}

    //public static void SetType(UIElement adornedElement, System.Type placeholderType)
    //{
    //    if (adornedElement == null)
    //        throw new ArgumentNullException("adornedElement");
    //    //offset = -adornedElement.Height;
    //    adornedElement.SetValue(TypeProperty, placeholderType);
    //}

    public static readonly DependencyProperty ShowDataContextProperty = DependencyProperty.RegisterAttached(
        "ShowDataContext", typeof(bool), typeof(System.Type), new FrameworkPropertyMetadata(false, OnChanged));

    public static readonly DependencyProperty ShowDimensionsProperty = DependencyProperty.RegisterAttached(
        "ShowDimensions", typeof(bool), typeof(System.Type), new FrameworkPropertyMetadata(false, OnChanged));

    public static void SetShowDataContext(UIElement adornedElement, bool value)
    {
        if (adornedElement == null)
        {
            throw new ArgumentNullException("adornedElement");
        }

        adornedElement.SetValue(ShowDataContextProperty, value);
    }

    public static void SetShowDimensions(UIElement adornedElement, bool value)
    {
        if (adornedElement == null)
        {
            throw new ArgumentNullException("adornedElement");
        }

        adornedElement.SetValue(ShowDimensionsProperty, value);
    }

    public static bool ShowDataContext(UIElement adornedElement)
    {
        if (adornedElement == null)
        {
            throw new ArgumentNullException("adornedElement");
        }

        return (bool)adornedElement.GetValue(ShowDataContextProperty);
    }

    public static bool ShowDimensions(UIElement adornedElement)
    {
        if (adornedElement == null)
        {
            throw new ArgumentNullException("adornedElement");
        }

        return (bool)adornedElement.GetValue(ShowDimensionsProperty);
    }

    //public static readonly DependencyProperty HighlightColourProperty = DependencyProperty.RegisterAttached(
    //    "HighlightColour", typeof(bool), typeof(System.Type), new FrameworkPropertyMetadata(false, OnChanged));

    //public static bool HighlightColour(UIElement adornedElement)
    //{
    //    if (adornedElement == null)
    //    {
    //        throw new ArgumentNullException("adornedElement");
    //    }

    //    return (bool)adornedElement.GetValue(HighlightColourProperty);
    //}

    //public static void SetHighlightColour(UIElement adornedElement, bool value)
    //{
    //    if (adornedElement == null)
    //    {
    //        throw new ArgumentNullException("adornedElement");
    //    }

    //    adornedElement.SetValue(HighlightColourProperty, value);
    //}

    private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement adornedElement)
        {
            throw new Exception("sdf234 dfg f,l,lgd");
        }

        if (adornedElement.IsLoaded)
        {
            AddAdorner(adornedElement, e);
        }
        else
        {
            adornedElement.Loaded += (s, e) => AddAdorner((FrameworkElement)sender, default);
        }

        static void AddAdorner(FrameworkElement adornedElement, DependencyPropertyChangedEventArgs e)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);

            bool flag = false;
            foreach (Adorner? adorner in adornerLayer.GetAdorners(adornedElement) ?? Array.Empty<Adorner>())
            {
                if (adorner is Text text)
                {
                    flag = true;
                    text.text = ToText(adornedElement);
                }
            }

            if (flag == false)
            {
                adornerLayer.Add(new Text(adornedElement, ToText(adornedElement), Dock.Bottom, Place.Inside));
            }
            else
            {
                Text.InvalidateTextAdorners(adornerLayer, adornedElement);
            }

            static string ToText(FrameworkElement adornedElement)
            {
                //System.Type? type = GetType(adornedElement);
                StringBuilder stringBuilder = new();
                if (ShowDataContext(adornedElement))
                {
                    stringBuilder.AppendLine(adornedElement.DataContext?.GetType().ToString());
                }
                if (ShowDimensions(adornedElement))
                {
                    stringBuilder.AppendLine("height: " + adornedElement.Height + "  x  width: " + adornedElement.Width);
                }

                return stringBuilder.ToString();
            }
        }
    }
}
