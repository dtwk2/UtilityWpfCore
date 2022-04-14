using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using Utility.WPF.Demo.Date.Infrastructure.Entity;
using Utility.WPF.Demo.Date.Infrastructure.ViewModel;
using UtilityHelperEx;
using UtilityWpf;

namespace Utility.WPF.Demo.Date;

public class DayNotesViewModel : ReactiveObject
{
    private readonly ObservableCollection<NoteViewModel> notes = new();

    //private readonly ICollection notes;
    private NoteViewModel? selectedNote;
    private int selectedIndex;



    public void Replace(ObservableCollection<NoteViewModel> notes, NoteViewModel selectedNote)
    {

        notes.SelectChanges()
            .Subscribe(a =>
            {
                if (a.Action == NotifyCollectionChangedAction.Add)
                    this.notes.AddRange(a.NewItems.OfType<NoteViewModel>().ToArray());
            });

        for (int i = this.notes.Count - 1; i >= 0; i--)
        {
            //await Task.Delay(50);
            this.notes.RemoveAt(i);
        }

        for (int i = notes.Count - 1; i >= 0; i--)
        {
            //await Task.Delay(50);
            this.notes.Add(notes[i]);
        }

        this.selectedNote = null;
        //await Task.Delay(500);
        this.selectedNote = selectedNote;

        this.WhenAnyValue(a => a.SelectedNote)
           .Subscribe(a => SelectedIndex = notes.IndexOf(a));

        this.WhenAnyValue(a => a.SelectedIndex)
           .Subscribe(a =>
           {
               if (a < 0)
               {
                   if (notes.Count > 0)
                       SelectedIndex = 0;
                   return;
               }

               if (notes.Count == 1)
               {
                   if (SelectedNote != default)
                       return;
                   SelectedNote = notes[0];
                   SelectedIndex = 0;
                   return;
               }

               if (notes.Count > a)
               {
                   SelectedNote = notes[a];
                   SelectedIndex = a;
               } //else {
                 //   SelectedNote = new[] { selectedNote }
                 //}
           });
    }

    public NoteViewModel SelectedNote
    {
        get => selectedNote;
        set => this.RaiseAndSetIfChanged(ref selectedNote, value);
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set => this.RaiseAndSetIfChanged(ref selectedIndex, value);
    }

    public ICollection Notes => notes;

    //public static DayNotesViewModel Instance => new();

}
