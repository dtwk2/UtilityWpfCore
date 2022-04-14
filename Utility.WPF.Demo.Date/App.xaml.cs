using System;
using System.Windows;
using Utility.Common;
using Utility.Persist;
using Utility.WPF.Demo.Date.Infrastructure.Entity;
using Utility.WPF.Demo.Date.Infrastructure.ViewModel;

namespace Utility.WPF.Demo.Date
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            FreeSqlFactory.InitialiseSQLite();
        }

        public static DateTime CurrentDate { get; set; }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveNote();
            base.OnExit(e);
        }

        void SaveNote()
        {
            if (CurrentDate != default)
                NotesViewModel.Instance.Save(Utility.WPF.Demo.Date.App.CurrentDate);
            //var map = AutoMapperSingleton.Instance.Map<NoteEntity>(DayNotesViewModel.Instance.SelectedNote);
            //map.Save();

        }
    }
}
