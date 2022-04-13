using DateWork.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Demo.Date.Infrastructure;
using Utility.WPF.Demo.Date.Infrastructure.Model;

namespace Utility.WPF.Demo.Date {
   public class NoteConverter : IValueConverter {
      //DayNotesViewModel? viewmodel = null;
      //Subject<Note[]> notesSubject = new();

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         //return new Ellipse { Fill = Brushes.Red, Height = 20, Width = 20 };
         if (value is not DateTime dateTime)
            return DependencyProperty.UnsetValue;

         var sDate = Note.GetTimeStamp(dateTime);
         var last = Notes.Current.Items
                                 .Where(a => a.Date == sDate)
                                 .OrderBy(a => a.RevisionDate)
                                 .LastOrDefault();

         return last ?? new Note { Date = Note.GetTimeStamp(dateTime), Text = "  " };
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}