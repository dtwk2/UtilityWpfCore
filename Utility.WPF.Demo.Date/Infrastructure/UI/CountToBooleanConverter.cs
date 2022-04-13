using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Demo.Date
{
    internal class CountToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int integer && integer > (parameter is int i ? i : 0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
