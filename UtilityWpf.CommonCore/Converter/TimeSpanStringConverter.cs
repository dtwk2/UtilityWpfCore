using System;
using System.Globalization;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    public class TimeSpanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                TimeSpan span = (TimeSpan)value;
                string formatted = string.Format("{0}{1}{2}{3}",
            span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
            span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
            span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
            span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

                if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

                if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

                return formatted;
            }
            else { return ""; }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is DateTime & values[1] is DateTime)
            {
                var ts = ((DateTime)values[0]) - ((DateTime)values[1]);

                switch (ts.Days)
                {
                    case (0):
                        return $"{ts.Hours }:{ts.Minutes}:{ts.Seconds}";

                    case (1):
                        return $"1 day { ts.Hours } hours";

                    default:
                        if (ts.Days > 1)
                            return $"{ts.Days} days {ts.Hours }:{ts.Minutes}:{ts.Seconds}";
                        else
                            return ((DateTime)values[0]).ToString("D", CultureInfo.CurrentCulture);
                }
            }
            else { return ""; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}