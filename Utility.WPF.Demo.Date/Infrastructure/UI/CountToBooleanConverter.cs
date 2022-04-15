using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Demo.Date {
   internal class CountToBooleanConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         var result = value is int integer && integer > (int.TryParse(parameter.ToString(), out int i) ? i : 0);
         return result;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
