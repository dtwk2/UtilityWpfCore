using DateWork.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Controls.Date.Model;
using Utility.WPF.Demo.Date.Infrastructure;
using Utility.WPF.Demo.Date.Infrastructure.Model;

namespace Utility.WPF.Demo.Date {
   public class NotesConverter : IValueConverter {
      DayNotesViewModel? viewmodel = null;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value == null)
            return DependencyProperty.UnsetValue;

         var date = (DateTime)value;
         if (viewmodel != null) {
            var vee = viewmodel.SelectedNote;
            var orderedNotes = Notes.Current.Items.Where(a => a.Date == viewmodel.SelectedNote.Date).OrderBy(a => a.RevisionDate).ToArray();
            var last = orderedNotes.LastOrDefault();

            if (last == null) {
               if (string.IsNullOrEmpty(vee.Text)) {
               }
               else {
                  SaveNote();
               }
            }
            else if (last.InitialText != vee.Text) {

               SaveNote();
            }

            void SaveNote() {
               var note = new Note { Date = viewmodel.SelectedNote.Date, InitialText = viewmodel.SelectedNote.Text, Text = viewmodel.SelectedNote.Text, RevisionDate = Note.GetTimeStamp(DateTime.Now) };
               // reset
               viewmodel.Reset();
               Notes.Current.Items.Add(note);
               Notes.Current.Save();
            }
         }

         var notes = NotesHelper.SelectNotes((DateTime)value).ToArray();
         viewmodel = new DayNotesViewModel(notes, notes.LastOrDefault() ?? new Note { Date = Note.GetTimeStamp(date) });
         return viewmodel;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}