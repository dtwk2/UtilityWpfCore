using System;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class GreaterThanConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int param = System.Convert.ToInt32(parameter);
            int val = System.Convert.ToInt32(value);

            return (param > val) == (!Invert);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}