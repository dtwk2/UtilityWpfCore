using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class EnumerableToCountConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return ((IEnumerable)value).Cast<object>().Count();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members
    }
}