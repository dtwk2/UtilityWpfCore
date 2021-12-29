﻿using System;
using System.Windows.Data;
using UtilityInterface.NonGeneric;
using UtilityWpf.Service;

namespace UtilityWpf.Converter
{
    // Converts enumerable's to a distinct list of given property's (parameter)  value
    [ValueConversion(typeof(string), typeof(IFilter))]
    public class StringToFilterConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new ContainsFilter((string)value, (string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? new object() : null;
        }

    }
}