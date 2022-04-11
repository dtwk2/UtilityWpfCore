using DateWork.Models;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Linq;

namespace Utility.WPF.Demo.Date
{
    public class DayNotesViewModel : ReactiveObject
    {
        private Note[] notes;

        //private readonly ICollection notes;
        private Note selectedNote;
        private int selectedIndex;

        public void Reset()
        {
            foreach (var note in notes)
            {
                note.Text = note.InitialText;
            }
        }

        public DayNotesViewModel(IObservable<Note[]> notes, Note selectedNote)
        {
            notes.Subscribe(n => Notes = n);
            this.selectedNote = selectedNote;

            this.WhenAnyValue(a => a.SelectedNote)
                .CombineLatest(notes)
                .Subscribe(ad =>
                {
                    var (a, notes) = ad;
                    SelectedIndex = Array.IndexOf(notes, a);
                });

            this.WhenAnyValue(a => a.SelectedIndex)
                  .CombineLatest(notes)
                  .Subscribe(ad =>
                  {
                      var (a, notes) = ad;

                      if (a < 0)
                      {
                          if (notes.Length > 0)
                              SelectedIndex = 0;
                          return;
                      }
                      if (notes.Length == 1)
                      {
                          SelectedNote = notes[0];
                          return;
                      }
                      SelectedNote = notes[a];
                  });

        }

        public DayNotesViewModel(Note[] notes, Note selectedNote)
        {
            this.notes = notes;
            this.selectedNote = selectedNote;

            this.WhenAnyValue(a => a.SelectedNote)
                .Subscribe(a =>
                {
                    SelectedIndex = Array.IndexOf(notes, a);
                });

            this.WhenAnyValue(a => a.SelectedIndex)
                .Subscribe(a =>
                {
                    if (a < 0)
                    {
                        if (notes.Length > 0)
                            SelectedIndex = 0;
                        return;
                    }
                    if (notes.Length == 1)
                    {
                        if (SelectedNote != default)
                            return;
                        SelectedNote = notes[0];
                        return;
                    }
                    SelectedNote = notes[a];
                });
        }

        public Note SelectedNote
        {
            get => selectedNote;
            set => this.RaiseAndSetIfChanged(ref selectedNote, value);
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set => this.RaiseAndSetIfChanged(ref selectedIndex, value);
        }

        public ICollection Notes
        {
            get => notes; set => this.RaiseAndSetIfChanged(ref notes, (Note[])value);
        }
    }
}