using DateWork.Helpers;
using DateWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.WPF.Controls.Date.Model
{
    internal class NotesHelper
    {
        public static IEnumerable<Note> SelectNotes(DateTime Day)
        {
            var notes = Notes.Current.Items;
            var dayNotes = notes.Where(a => !a.IsMonthDay
                && MonthDayHelper.IsSameMonthDay(Note.SetTimeStamp(a.Date), Day));
            var monthDayNotes = notes.Where(a => a.IsMonthDay
                && MonthDayHelper.IsSameMonthMonthMonthDay(Note.SetTimeStamp(a.Date), Day)); ;
            return dayNotes.Concat(monthDayNotes);
        }
    }
}
