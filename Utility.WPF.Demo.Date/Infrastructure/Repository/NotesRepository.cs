using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ReactiveUI;
using Utility.Common;
using Utility.WPF.Controls.Date.Model;
using Utility.WPF.Demo.Date.Infrastructure.Entity;
using Utility.WPF.Demo.Date.Infrastructure.ViewModel;

namespace Utility.WPF.Demo.Date.Infrastructure.Repository
{
    internal class NotesRepository
    {
        readonly Dictionary<DateTime, ObservableCollection<NoteViewModel>> notes = new();

        public NoteViewModel FindMostRecent(DateTime date)
        {
            NoteViewModel noteViewModel = new() { Date = date };

            if (notes.ContainsKey(date.Date) == false)
            {
                notes[date] = new ObservableCollection<NoteViewModel> { noteViewModel };
            }

            _ = NoteEntity
                .Where(a => a.Date == date)
                .OrderByDescending(a => a.CreateTime)
                .FirstAsync()
                .ToObservable()
                .WhereNotNull()
                .Take(1)
                .Subscribe(single =>
                {
                    var map = AutoMapperSingleton.Instance.Map<NoteViewModel>(single);
                    noteViewModel.Text = map.Text;
                    noteViewModel.CreateTime = map.CreateTime;
                    noteViewModel.Id = map.Id;
                    notes[date].InsertInOrderIfMissing(noteViewModel);

                });

            return noteViewModel;
        }

        public async Task<ObservableCollection<NoteViewModel>> FindAllAsync(DateTime date)
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
                        notes[date].InsertInOrderIfMissing(noteViewModel);
                    }
                    else
                    {
                        var map = AutoMapperSingleton.Instance.Map<NoteViewModel[]>(list);
                        notes[date].InsertInOrderIfMissing(map);
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
                    // no need to insert if text hasn't changed
                    return null;
                }
                else
                {
                    // not possible to insert successfully if id matches one already in the database
                    map.Id = default;
                }
            var insert = await map.InsertAsync();
            var mapReverse = AutoMapperSingleton.Instance.Map<NoteViewModel>(dayNotes.Last());
            dayNotes.Add(mapReverse);
            return insert;
        }

        public static NotesRepository Instance { get; } = new();
    }
}
