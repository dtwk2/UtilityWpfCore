using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;


namespace UtilityWpf.Converter
{
    public class TenseToDateTimeConverter : IValueConverter
    {
        public int Days { get; set; } = 14;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)((dynamic)value).IsChecked switch
            {
                true => DateTime.Now.GetRange(DateTime.Now.AddDays(Days)).ToList(),
                false => DateTime.Now.AddDays(-Days).GetRange(DateTime.Now).ToList(),
                _ => new[] { DateTime.Now },
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Get range of dates between the startdate and enddate
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    static class Helper
    {
        public static IEnumerable<DateTime> GetRange(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (int)(endDate - startDate).TotalDays + 1).Select(i => startDate.AddDays(i));
        }
    }
}
