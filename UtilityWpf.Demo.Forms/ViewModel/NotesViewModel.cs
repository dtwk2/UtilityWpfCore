using Endless;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using UtilityHelperEx;
using _ViewModel = Utility.ViewModel.ViewModel;

namespace UtilityWpf.Demo.Forms.ViewModel
{
    public class NotesViewModel : _ViewModel
    {
        private ICommand changeCommand;

        public NotesViewModel(string header, IReadOnlyCollection<string> enumerable) : this(header, enumerable.Select(a => new NoteViewModel(a)).ToArray())
        {
        }

        public NotesViewModel(string header, IReadOnlyCollection<NoteViewModel> collection) : base(header)
        {
            Collection = new ObservableCollection<NoteViewModel>(collection);
            Intialise();
        }

        public NotesViewModel(string header) : this(header, Array.Empty<NoteViewModel>())
        {
        }

        public override ObservableCollection<NoteViewModel> Collection { get; }

        public System.Collections.IEnumerator NewItem { get => 0.Repeat().Select(a => new NoteViewModel(string.Empty)).GetEnumerator(); }

        public ICommand ChangeCommand => changeCommand ??= ReactiveCommand.Create<object, Unit>(Change);

        private Unit Change(object xx)
        {
            //if (xx is CollectionEventArgs { Item: string item })
            //{
            //    var ivm = new NoteViewModel(item);
            //    //Subscribe(ivm);
            //    Collection.Add(ivm);
            //}
            return Unit.Default;
        }
    }
}