using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace UtilityWpf.Converter
{
    public class HighestVisualParentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value?.GetType();

            var aa = (value as FrameworkElement)?.GetSelfAndAncestors() ?? Array.Empty<DependencyObject>();
            var se = type != null ? aa.Where(a => a.GetType() == type) : Array.Empty<DependencyObject>();

            return se.LastOrDefault() ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public static HighestVisualParentConverter Instance => new HighestVisualParentConverter();
    }
}