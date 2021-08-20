using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace UtilityWpf
{
    public class IsLastItemSelectedConverter : IMultiValueConverter
    {

        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value[1] is null || value[1]== DependencyProperty.UnsetValue)
            {
                return true != Invert;
            }
            if (value[0] is ItemsControl selector && value[1] is int index)
            {
                int count = selector.Items.Count;
                return (index == -1 || index == count - 1) != Invert; 
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public bool Invert { get; set; }

    }
}