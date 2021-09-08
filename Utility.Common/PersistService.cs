using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using UtilityHelperEx;
using System.Reactive.Linq;
using UtilityInterface.NonGeneric.Database;
using System.Reactive.Subjects;
using DynamicData;
using System.Windows.Input;
using System.ComponentModel;
using System.Reactive.Disposables;
//using UtilityWpf.Model;
using UtilityHelper.NonGeneric;
using UtilityWpf;
using System.Collections.ObjectModel;
using UtilityWpf.Common;

namespace Utility.Common
{

    public record CollectionChangeMessage(IEnumerable<object> Objects);
    public record RepositoryMessage(IDatabaseService Service);

    public class PersistService : IObserver<RepositoryMessage>, IObservable<CollectionChangeMessage>, IDisposable
    {
        private IDisposable? disposable;
        private readonly ReplaySubject<RepositoryMessage> repositoryMessages = new(1);
        private readonly ReplaySubject<CollectionChangeMessage> collectionChangeMessages = new(1);

        public ObservableRangeCollection<object> Items { get; } = new();

        public PersistService(bool initialise = true)
        {
            if (initialise)
                Init();
        }

        protected void Init()
        {
            var dis1 = NewItems().Merge(OldItems())
              .Scan(default((CollectionChange, CollectionChange)), (a, b) => (a.Item2, b))
              .WithLatestFrom(repositoryMessages.Select(a => a.Service).WhereNotDefault())
              .Subscribe(cc =>
              {
                  var (((action, first), (second, third)), docstore) = cc;
                  if (action == NotifyCollectionChangedAction.Reset)
                      return;

                  switch (second)
                  {
                      case NotifyCollectionChangedAction.Reset:
                          {
                              return;
                          }
                      case NotifyCollectionChangedAction.Replace:
                          {
                              foreach (var item in third)
                                  docstore.Update(item);
                              break;
                          }

                      case NotifyCollectionChangedAction.Add:
                          {
                              foreach (var item in third)
                                  docstore.Insert(item);
                              break;
                          }

                      case NotifyCollectionChangedAction.Remove:
                          {
                              foreach (var item in third)
                                  docstore.Delete(item);
                              break;
                          }
                  }
                  var count = docstore.SelectAll().Count();
                  if (count != Items.Count)
                  {
                      //TODO add logging
                  }
                  //else if (reason == NotifyCollectionChangedAction.)
                  //{
                  //    _docstore.Upsert(item);
                  //}
              });

            var changeSet = ObservableChangeSet.Create<object, string>(observer =>
            {
                return repositoryMessages.Select(a => a.Service).WhereNotDefault()
                   .Select(a => a.SelectAll().Cast<object>())
                   .Subscribe(objects =>
                   {
                       if (objects.Any() == false)
                           return;
                       foreach (var change in objects.OfType<INotifyPropertyChanged>())
                       {
                           change.Changes()
                           .Subscribe(x =>
                           {
                               observer.AddOrUpdate(x);

                           });
                       }
                       observer.Edit(a =>
                       {
                           a.Clear();
                           foreach (var change in objects.OfType<INotifyPropertyChanged>())
                           {
                               a.AddOrUpdate(change);
                           }
                       });
                       //CollectionChangeCommand?.Execute(observer.Items);
                       collectionChangeMessages.OnNext(new CollectionChangeMessage(observer.Items.ToArray()));

                       Items.ReplaceWithRange(objects);
                   });


            }, a => a.ToString());


            var dis2 = changeSet
                .OnItemUpdated((a, sd) =>
                {

                    Items.Replace(a, sd);
                })
                .Subscribe(a =>
                {

                });

         //   changeSet.Bind(out ReadOnlyObservableCollection<object>? aa);

            disposable = new CompositeDisposable(dis1, dis2);          
        }

        private IObservable<CollectionChange> OldItems()
        {
            return Items
                .SelectChanges()
                .Select(a => new CollectionChange(a.Action, a.OldItems?.Cast<object>()?.ToArray() ?? Array.Empty<object>()));
        }

        private IObservable<CollectionChange> NewItems()
        {
            return Items
                .SelectChanges()
                .Select(a => new CollectionChange(a.Action, a.NewItems?.Cast<object>()?.ToArray() ?? Array.Empty<object>()));
        }

        public void Dispose()
        {
            disposable?.Dispose();
        }

        //private static void RepositoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        //{
        //    (dependencyObject as PersistBehavior).repositoryChanges.OnNext(e.NewValue as IDatabaseService);
        //}

        //private void Change_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        public IDisposable Subscribe(IObserver<CollectionChangeMessage> observer)
        {
            return collectionChangeMessages.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RepositoryMessage value)
        {
            repositoryMessages.OnNext(value);
        }
    }

    internal struct CollectionChange
    {
        public NotifyCollectionChangedAction Action;
        public IEnumerable<object> Item2;

        public CollectionChange(NotifyCollectionChangedAction action, IEnumerable<object> item2)
        {
            Action = action;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return obj is CollectionChange other &&
                   Action == other.Action &&
                   EqualityComparer<IEnumerable<object>>.Default.Equals(Item2, other.Item2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Action, Item2);
        }

        public void Deconstruct(out NotifyCollectionChangedAction action, out IEnumerable<object> item2)
        {
            action = Action;
            item2 = Item2;
        }

        public static implicit operator (NotifyCollectionChangedAction Action, IEnumerable<object>)(CollectionChange value)
        {
            return (value.Action, value.Item2);
        }

        public static implicit operator CollectionChange((NotifyCollectionChangedAction Action, IEnumerable<object>) value)
        {
            return new CollectionChange(value.Action, value.Item2);
        }
    }
}
