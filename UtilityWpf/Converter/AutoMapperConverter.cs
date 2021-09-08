using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Common;

namespace UtilityWpf.Converter
{
    public class AutoMapperConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapped= AutoMapperSingleton.Instance.Map(value, value.GetType(), parameter as Type);
            return mapped;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static AutoMapperConverter Instance { get; } = new AutoMapperConverter();
    }
}
