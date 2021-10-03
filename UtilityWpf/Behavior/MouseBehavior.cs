﻿using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using UtilityWpf.Attached;

namespace UtilityWpf.Behavior
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/45608326/c-sharp-wpf-border-style-animations"></a>
    /// </summary>
    public class MouseBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(MouseBehavior), new PropertyMetadata(null));

        public MouseBehavior()
        {
        }

        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDown += Element_PreviewMouseDown;
            AssociatedObject.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            AssociatedObject.MouseDown += element_MouseDown;
            AssociatedObject.MouseUp += element_MouseUp;
            AssociatedObject.MouseLeave += element_MouseLeave;
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
        }


        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= Element_PreviewMouseDown;
            AssociatedObject.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
            AssociatedObject.MouseDown -= element_MouseDown;
            AssociatedObject.MouseUp -= element_MouseUp;
            AssociatedObject.MouseLeave -= element_MouseLeave;
            AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
        }

        private void AssociatedObject_MouseEnter(object sender, MouseEventArgs e)
        {
            AsElement(sender).SetValue(Ex.IsMouseOverProperty, true);
        }

        private void Element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AsElement(sender).SetValue(Ex.IsPressedProperty, true);
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AsElement(sender).SetValue(Ex.IsPressedProperty, false);
        }

        void element_MouseLeave(object sender, MouseEventArgs e)
        {
            AsElement(sender).SetValue(Ex.IsPressedProperty, false);
            AsElement(sender).SetValue(Ex.IsMouseOverProperty, false);
        }

        void element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AsElement(sender).SetValue(Ex.IsPressedProperty, false);
        }
        void element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AsElement(sender).SetValue(Ex.IsPressedProperty, true);
        }

        private FrameworkElement AsElement(object sender)
        {
            if (Element is FrameworkElement elem)
            {
                return elem;
            }
            if (sender is not FrameworkElement element)
            {
                throw new Exception("s2222dfsd");
            }
            return element;
        }
    }
}
