using System.Windows;
using Utility.Persist;

namespace Utility.WPF.Demo.Date {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {

      public App() {
         FreeSqlFactory.InitialiseSQLite();
      }

      protected override void OnExit(ExitEventArgs e) {


         base.OnExit(e);
      }

      void SaveNote() {
         DayNotesViewModel.Instance.SelectedNote.SaveAsync();
         //var note = new NoteViewModel { Date = viewmodel.SelectedNoteViewModel.Date, InitialText = viewmodel.SelectedNoteViewModel.Text, Text = viewmodel.SelectedNoteViewModel.Text, RevisionDate = NoteViewModel.GetTimeStamp(DateTime.Now) };
         //// reset
         //viewmodel.Reset();
         //Notes.Current.Items.Add(note);
         //Notes.Current.Save();
      }
   }
}
