using DateWork.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Demo.Date.Infrastructure
{
    public class NoteDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return DependencyProperty.UnsetValue;
            return Note.SetTimeStamp(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Note.GetTimeStamp((DateTime)value);
        }
    }
}