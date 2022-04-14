using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Demo.Date.Infrastructure.ViewModel;

namespace Utility.WPF.Demo.Date
{
    public class NotesConverter : IValueConverter
    {
        //private DateTime date;

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime date)
                return DependencyProperty.UnsetValue;

            if (App.CurrentDate != default)
                NotesViewModel.Instance.Save(App.CurrentDate);

            App.CurrentDate = date;

            var notesViewModel = new DayNotesViewModel();

            var _ = NotesViewModel.Instance.FindAsync(date)
                .ToObservable()
                .Subscribe(notes =>
                {
                    notesViewModel.Replace(notes, notes.Last());
                });

            return notesViewModel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}