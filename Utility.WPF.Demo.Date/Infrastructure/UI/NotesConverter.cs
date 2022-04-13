using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Demo.Date.Infrastructure;
using Utility.WPF.Demo.Date.Infrastructure.Entity;

namespace Utility.WPF.Demo.Date {
   public class NotesConverter : IValueConverter {
      private DayNotesViewModel viewmodel;

      public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) {
         if (value == null)
            return DependencyProperty.UnsetValue;


         var orderedNotes = viewmodel?
            .Notes
            .OfType<NoteEntity>()
            .OrderByDescending(a => a.CreateTime)
            .FirstOrDefault();

         var last = orderedNotes;

         if (last != null) {
            if (string.IsNullOrEmpty(last.Text)) {
            }
            else {
               SaveNote();
            }
         }

         var date = (DateTime)value;
         //if (viewmodel != null) {
         viewmodel = DayNotesViewModel.Instance;
         var vee = viewmodel.SelectedNote;
         var selectedDate = viewmodel.SelectedNote?.Date;

         // if (selectedDate == default) {


         var notes = NotesHelper.SelectNotes((DateTime)value).Result;
         if (notes.Any()) {
            viewmodel.Replace(notes, notes.Last());
         }
         else {
            var nt = new NoteEntity { Date = date, Text = string.Empty };
            viewmodel.Replace(new[] { nt }, nt);
         }



         return viewmodel;


         void SaveNote() {
            viewmodel.SelectedNote.SaveAsync();
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}