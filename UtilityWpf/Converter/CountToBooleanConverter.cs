using System;
using System.Windows.Data;

namespace UtilityWpf
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class CountToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (int.TryParse(parameter.ToString(), out int param))
                return (param >= (int)value) != Invert;
            return (0 == (int)value) != Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public bool Invert { get; set; }

        #endregion IValueConverter Members
    }
}