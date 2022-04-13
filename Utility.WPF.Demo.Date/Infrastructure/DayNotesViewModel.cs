using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DynamicData;
using Utility.WPF.Demo.Date.Infrastructure.Entity;

namespace Utility.WPF.Demo.Date;

public class DayNotesViewModel : ReactiveObject {
   private readonly ObservableCollection<NoteEntity> notes = new();

   //private readonly ICollection notes;
   private NoteEntity selectedNote;
   private int selectedIndex;



   public void Replace(NoteEntity[] notes, NoteEntity selectedNote) {

      for (int i = this.notes.Count - 1; i >= 0; i--) {
         //await Task.Delay(50);
         this.notes.RemoveAt(i);
      }

      for (int i = notes.Length - 1; i >= 0; i--) {
         //await Task.Delay(50);
         this.notes.Add(notes[i]);
      }

      this.selectedNote = null;
      //await Task.Delay(500);
      this.selectedNote = selectedNote;

      this.WhenAnyValue(a => a.SelectedNote)
         .Subscribe(a => { SelectedIndex = Array.IndexOf(notes, a); });

      this.WhenAnyValue(a => a.SelectedIndex)
         .Subscribe(a => {
            if (a < 0) {
               if (notes.Length > 0)
                  SelectedIndex = 0;
               return;
            }

            if (notes.Length == 1) {
               if (SelectedNote != default)
                  return;
               SelectedNote = notes[0];
               SelectedIndex = 0;
               return;
            }

            if (notes.Length > a) {
               SelectedNote = notes[a];
               SelectedIndex = a;
            } //else {
            //   SelectedNote = new[] { selectedNote }
            //}
         });
   }

   public NoteEntity SelectedNote {
      get => selectedNote;
      set => this.RaiseAndSetIfChanged(ref selectedNote, value);
   }

   public int SelectedIndex {
      get => selectedIndex;
      set => this.RaiseAndSetIfChanged(ref selectedIndex, value);
   }

   public ICollection Notes {
      get => notes;
   }

   public static DayNotesViewModel Instance => new();

}
