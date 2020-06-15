using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilityWpf.View
{
    public class ValidationForm : ContentControl
    {
        static ValidationForm()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationForm), new FrameworkPropertyMetadata(typeof(ValidationForm)));
        }

        public ValidationForm()
        {
            Content = new Rectangle { Height = 300, Width = 300, Fill = Brushes.Gainsboro }; ;
        }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register("IsValid", typeof(bool), typeof(ValidationForm), new PropertyMetadata(false));


    }
}
