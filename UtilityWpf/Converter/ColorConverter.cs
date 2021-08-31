using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UtilityWpf.Converter
{

    [ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value is System.Drawing.Color color ?
                color.ToUIColor() :
                 parameter is System.Windows.Media.Color defaultColour ?
                 defaultColour :
                 DependencyProperty.UnsetValue;
            return val;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ColorConverter Instance { get; } = new ColorConverter();
    }

    public static class ColorConverterHelper
    {
        public static Color ToUIColor(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}