﻿using System;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? new object() : null;
        }

    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class InverseNullToBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return (bool)value ? new object() : null;
        }

        public static InverseNullToBooleanConverter Instance { get; } = new InverseNullToBooleanConverter();

    }
}