﻿using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;
using UtilityHelper;
using UtilityHelper.NonGeneric;

namespace UtilityWpf.Converter
{
    // Converts enumerable's to a distinct list of given property's (parameter)  value
    [ValueConversion(typeof(IEnumerable), typeof(IEnumerable))]
    public class EnumerableToDistinctPropertiesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                if ((value as IEnumerable).Count() > 0)
                    return (value as IEnumerable).Cast<object>()
                        .GetPropertyValues<object>((string)parameter)
                        .Distinct()
                        .OrderBy(a => a)
                        .ToList();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members
    }
}