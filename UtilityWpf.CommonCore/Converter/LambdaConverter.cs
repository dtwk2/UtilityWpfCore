using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UtilityWpf.Converter
{
    public static class LambdaConverter
    {
        public static IValueConverter PathToNameConverter => LambdaConverters.ValueConverter.Create<string, string>(e =>
         {
             return System.IO.Path.GetFileNameWithoutExtension(e.Value);
         });
    }
}
