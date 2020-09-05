using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UtilityWpf.DemoApp;

namespace UtilityWpf.Interactive.Demo.Common
{
    public class ColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Character character)
            {
                return character.Color;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}