using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilityWpf.View
{
    public class ValidationForm : Control
    {
        static Rectangle Rectangle = new System.Windows.Shapes.Rectangle { Height = 300, Width = 300, Fill = Brushes.Gainsboro };

        static ValidationForm()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationForm), new FrameworkPropertyMetadata(typeof(ValidationForm)));
        }

        public ValidationForm()
        {
            Content = Rectangle;
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(ValidationForm));
        
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register("IsValid", typeof(bool), typeof(ValidationForm), new PropertyMetadata(false));


    }
}
