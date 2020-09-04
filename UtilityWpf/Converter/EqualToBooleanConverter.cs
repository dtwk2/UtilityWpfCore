using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;



namespace UtilityWpf.Converter
{
    public class EqualToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EqualToBooleanConverter Instance => new EqualToBooleanConverter();

        #endregion IValueConverter Members
    }

    public class EqualToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EqualToVisibilityConverter Instance => new EqualToVisibilityConverter();

        #endregion IValueConverter Members
    }
}