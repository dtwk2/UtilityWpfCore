using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Demo.Date.Infrastructure.ViewModel;

namespace Utility.WPF.Demo.Date
{
    public class NoteConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return new Ellipse { Fill = Brushes.Red, Height = 20, Width = 20 };
            if (value is not DateTime dateTime)
                return DependencyProperty.UnsetValue;

            return NotesViewModel.Instance.FindMostRecent(dateTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}