using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    public class TypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value.GetType();
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EnumConverter Instance { get; } = new EnumConverter();
    }
}