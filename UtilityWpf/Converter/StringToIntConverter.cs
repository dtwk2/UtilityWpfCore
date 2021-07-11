using System;
using System.Windows.Data;
using UtilityWpf.Service;

namespace UtilityWpf.Converter
{
    [ValueConversion(typeof(string), typeof(int))]
    public class StringToIntConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new ContainsFilter((string)value, (string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? new object() : null;
        }

        public static StringToIntConverter Instance { get; } = StringToIntConverter.Instance;

        #endregion IValueConverter Members
    }
}