using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Endless;

namespace UtilityWpf.Demo.Sandbox.ViewModel
{
    public class NotesViewModel
    {
        private ICommand changeCommand;

        public string Header { get; } = "NotesViewModel";

        public ObservableCollection<NoteViewModel> Collection { get; } = new ObservableCollection<NoteViewModel> {};

        public System.Collections.IEnumerator NewItem { get => 0.Repeat().Select(a=> new NoteViewModel(a.ToString())).GetEnumerator(); }

        public ICommand ChangeCommand => changeCommand ??= new ActionCommand(Change);

        private void Change()
        {
        }
    }

    public class NoteViewModel : ReactiveObject
    {
        private string text;

        public NoteViewModel(string text)
        {
            Text = text;
        }

        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }
    }
}
