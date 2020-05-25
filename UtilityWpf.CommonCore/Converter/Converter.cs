using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    public sealed class DefaultConverter : IValueConverter
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DefaultConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static DefaultConverter Instance { get; }= new DefaultConverter();
    }
}
