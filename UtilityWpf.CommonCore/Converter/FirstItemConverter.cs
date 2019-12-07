using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf
{
    public class FirstItemConverter : IMultiValueConverter
    {
        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    int count = (int)values[2];

        //    if (values != null && values.Length == 3 && count > 0)
        //    {
        //        System.Windows.Controls.ItemsControl itemsControl = values[0] as System.Windows.Controls.ItemsControl;
        //        var itemContext = (values[1] as System.Windows.Controls.ContentPresenter).DataContext;

        //        var lastItem = itemsControl.Items[0];

        //        return Equals(lastItem, itemContext);
        //    }

        //    return DependencyProperty.UnsetValue;
        //}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ItemsControl itemscontrol = values[0] as ItemsControl;
            int count = itemscontrol.Items.Count;

            if (values != null && values.Length == 2 && count > 0)
            {
                var itemContext = (values[1] as System.Windows.Controls.ContentPresenter).DataContext;
                var lastItem = itemscontrol.Items[0];
                return Equals(lastItem, itemContext);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}