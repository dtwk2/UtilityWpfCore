using System;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    public static class LambdaConverter
    {
        public static IValueConverter PathToNameConverter => LambdaConverters.ValueConverter.Create<string, string>(e =>
         {
             return System.IO.Path.GetFileNameWithoutExtension(e.Value);
         });

        public static IValueConverter SecondsToDateTimeConverter => LambdaConverters.ValueConverter.Create<double, DateTime>(e =>
        {
            return DateTime.UnixEpoch + TimeSpan.FromSeconds(e.Value);
        });

        public static IValueConverter DaysToDateTimeConverter => LambdaConverters.ValueConverter.Create<double, DateTime>(e =>
        {
            return DateTime.UnixEpoch + TimeSpan.FromDays(e.Value);
        });
    }
}