using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using DynamicData;
using DynamicData.Binding;
using LanguageExt;
using ReactiveUI;
using Utility.Common;
using Utility.WPF.Demo.Date.Infrastructure.Entity;

namespace Utility.WPF.Demo.Date.Infrastructure.ViewModel
{
    public sealed class NotesViewModel
    {
        private static readonly NotesViewModel instance = new();
        readonly Dictionary<DateTime, ObservableCollection<NoteViewModel>> notes = new();

        public NotesViewModel()
        {
        }

        //public ICollection<NoteViewModel> Find(DateTime date)
        //{
        //    if (notes.ContainsKey(date.Date))
        //    {
        //        return notes[date];
        //    }

        //    var tempNotes = notes[date] = new ObservableCollection<NoteViewModel> { new() { Date = date } };
        //    //notes.Add(date, new List<NoteViewModel> { noteViewModel });
        //    var _ = NoteEntity
        //        .Where(a => a.Date == date)
        //        .OrderByDescending(a => a.CreateTime)
        //        .ToListAsync()
        //        .ToObservable()
        //        .WhereNotNull()
        //        .Subscribe(async a =>
        //        {
        //            notes[date].Clear();
        //            foreach (var x in a)
        //            {
        //                await Task.Delay(50);
        //                var map = AutoMapperSingleton.Instance.Map<NoteViewModel>(x);
        //                notes[date].Add(map);
        //            }
        //        });

        //    return tempNotes;
        //}

        public NoteViewModel FindMostRecent(DateTime date)
        {
            //if (date == DateTime.Now.Date.AddDays(1))
            //{

            //}
            if (notes.ContainsKey(date.Date))
            {
                return notes[date].Last();
            }

            NoteViewModel noteViewModel = new() { Date = date };
            notes[date] = new ObservableCollection<NoteViewModel> { noteViewModel };
            //notes.Add(date, new List<NoteViewModel> { noteViewModel });
            var _ = NoteEntity
                .Where(a => a.Date == date)
                .OrderByDescending(a => a.CreateTime)
                .FirstAsync()
                .ToObservable()
                .WhereNotNull()
                .Subscribe(async single =>
                {
                    var map = AutoMapperSingleton.Instance.Map<NoteViewModel>(single);
                    noteViewModel.Text = map.Text;
                    noteViewModel.CreateTime = map.CreateTime;
                    noteViewModel.Id = map.Id;
                    //notes[date].ReplaceOrAdd(noteViewModel, map);
                });

            return noteViewModel;
        }

        public async Task<ObservableCollection<NoteViewModel>> FindAsync(DateTime date)
        {
            if (notes.ContainsKey(date))
            {
                return await Observable.Return(notes[date]).ToTask();
            }

            notes[date] = new ObservableCollection<NoteViewModel>();

            return await NoteEntity
                .Where(a => a.Date == date)
                .OrderByDescending(a => a.CreateTime)
                .ToListAsync()
                .ToObservable()
                .Select(list =>
                {
                    if (list == null)
                    {
                        var noteViewModel = new NoteViewModel { Date = date };
                        //noteViewModel.WhenAnyValue(a => a.Text)
                        //    .Subscribe(async a =>
                        //    {
                        //        var find = await NoteEntity.FindAsync(noteViewModel.Id);
                        //        if (find == null)
                        //        {
                        //            var map = AutoMapperSingleton.Instance.Map<NoteEntity>(find);
                        //            NoteEntity.Orm.Insert(map);
                        //    });
                        notes[date].Add(noteViewModel);
                    }
                    else
                    {
                        var map = AutoMapperSingleton.Instance.Map<NoteViewModel[]>(list);
                        notes[date].AddRange(map);
                    }

                    return notes[date];
                }).ToTask();

        }

        public async Task<NoteEntity?> Save(DateTime date)
        {
            var dayNotes = notes[date];
            var map = AutoMapperSingleton.Instance.Map<NoteEntity>(dayNotes.Last());
            if (await NoteEntity.FindAsync(map.Id) is { Text: var text })
                if (text == map.Text)
                {
                    return null;
                }
                else
                {
                    map.Id = default;

                }
            var insert = await map.InsertAsync();
            var map2 = AutoMapperSingleton.Instance.Map<NoteViewModel>(dayNotes.Last());
            dayNotes.Add(map2);
            return insert;
        }

        public ICollection Notes => notes;

        public static NotesViewModel Instance => instance;


    }
}
