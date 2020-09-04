using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace UtilityWpf.ViewModel
{
    public class GroupMasterPropertyChangedViewModel<T, R, S> where T : INotifyPropertyChanged
    {

        public GroupMasterPropertyChangedViewModel(IObservable<IChangeSet<T, R>> changeSet, Expression<Func<T, S>> func)
        {
            Collection = GroupHelper.Convert(changeSet.GroupOnProperty(func), CreateViewModel);
        }



        public ReadOnlyObservableCollection<GroupViewModel<T, R, S>> Collection { get; }




        public virtual GroupViewModel<T, R, S> CreateViewModel(IGroup<T, R, S> group)
        {
            return new GroupViewModel<T, R, S>(group);
        }
    }



    public class GroupMasterViewModel<T, R, S>
    {

        public GroupMasterViewModel(IObservable<IGroupChangeSet<T, R, S>> groups)
        {
            Collection = GroupHelper.Convert(groups, CreateViewModel);
        }

        public GroupMasterViewModel(IObservable<IChangeSet<T, R>> changeSet, Func<T, S> func)
        {
            Collection = GroupHelper.Convert(changeSet.Group(func), CreateViewModel);
        }

        public ReadOnlyObservableCollection<GroupViewModel<T, R, S>> Collection { get; }

        public virtual GroupViewModel<T, R, S> CreateViewModel(IGroup<T, R, S> group)
        {
            return new GroupViewModel<T, R, S>(group);
        }
    }



    public class GroupViewModel<T, R, S> : ReactiveObject
    {
        private int count;
        private IReadOnlyCollection<T> collection;

        public S Key { get; private set; }

        public int Count => count;

        public IReadOnlyCollection<T> Collection => collection;

        public GroupViewModel(IGroup<T, R, S> group)
        {
            Key = group.Key;
       
            group.Cache.Connect().ToCollection()

               .Subscribe(a =>
               {
                   this.RaiseAndSetIfChanged(ref count, a.Count, nameof(Count));
                   this.RaiseAndSetIfChanged(ref collection, a, nameof(Collection));
               },
               e =>
               {
               });
        }
    }


    internal static class GroupHelper
    {
        public static ReadOnlyObservableCollection<GroupViewModel<T, R, S>> Convert<T, R, S>(IObservable<IGroupChangeSet<T, R, S>> groups, Func<IGroup<T, R, S>, GroupViewModel<T, R, S>> createFunc)
        {
            groups
      .Transform(createFunc)
      .ObserveOnDispatcher()
      .Bind(out var data)
      //.DisposeMany()
      .Subscribe(v =>
      {
      });

            return data;
        }
    }
}
