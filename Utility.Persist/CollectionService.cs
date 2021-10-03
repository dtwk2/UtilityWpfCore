using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using UtilityHelperEx;
using System.Reactive.Linq;
using UtilityInterface.NonGeneric.Data;
using System.Reactive.Subjects;
using DynamicData;
using System.ComponentModel;
using System.Reactive.Disposables;
using UtilityHelper.NonGeneric;
using UtilityWpf;
using UtilityWpf.Common;
using Utility.Common.Helper;

namespace Utility.Persist
{
    public record CollectionChangeMessage(CollectionChange change);
    public record RepositoryMessage(IRepository Service);

    public class CollectionService : IObserver<RepositoryMessage>, IObservable<CollectionChangeMessage>, IDisposable
    {
        private IDisposable disposable;
        private readonly ReplaySubject<RepositoryMessage> repositoryMessages = new(1);
        private readonly ReplaySubject<CollectionChangeMessage> collectionChangeMessages = new();

        public ObservableRangeCollection<object> Items { get; } = new();

        public CollectionService(bool initialise = true)
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
                  var ((a, b), repository) = cc;
           
                  if (a?.Action == NotifyCollectionChangedAction.Reset)
                      return;

                  switch (b.Action)
                  {
                      case NotifyCollectionChangedAction.Reset:
                          {
                              return;
                          }
                      case NotifyCollectionChangedAction.Replace:
                          {
                              foreach (var item in b.Items)
                                  repository.Update(item);
                              break;
                          }

                      case NotifyCollectionChangedAction.Add:
                          {
                              foreach (var item in b.Items)
                                  repository.Add(item);
                              break;
                          }
                      case NotifyCollectionChangedAction.Remove:
                          {
                              foreach (var item in b.Items)
                                  repository.Remove(item);
                              break;
                          }
                  }
                  var count = repository.Count();
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
                   .Select(a => a.FindAll<object>())
                   .Subscribe(objects =>
                   {
                       if (objects.Any() == false)
                           return;
                       foreach (var change in objects.OfType<INotifyPropertyChanged>())
                       {
                           change
                           .Changes()
                           .Subscribe(x =>
                           {
                               observer.AddOrUpdate(x.source);

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
                       collectionChangeMessages.OnNext(new CollectionChangeMessage(new CollectionChange(NotifyCollectionChangedAction.Add, observer.Items.ToArray())));

                       Items.ReplaceWithRange(objects);
                   });


            }, a => a.ToString());


            var dis2 = changeSet
                .OnItemUpdated((a, sd) =>
                {
                    if (Items.Count > 0)
                    {
                        Items.Replace(a, sd);
                    }
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
        //    (dependencyObject as PersistBehavior).repositoryChanges.OnNext(e.NewValue as IRepository);
        //}

        //private void Change_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        public IDisposable Subscribe(IObserver<CollectionChangeMessage> observer)
        {

            observer.OnNext(new(new(NotifyCollectionChangedAction.Add, Items)));
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

    public record CollectionChange
    {
        public NotifyCollectionChangedAction Action { get; }
        public IEnumerable<object> Items { get; }

        public CollectionChange(NotifyCollectionChangedAction action, IEnumerable<object> item2)
        {
            Action = action;
            Items = item2;
        }

        public void Deconstruct(out NotifyCollectionChangedAction action, out IEnumerable<object> item)
        {
            action = Action;
            item = Items;
        }

        public static implicit operator (NotifyCollectionChangedAction Action, IEnumerable<object>)(CollectionChange value)
        {
            return (value.Action, value.Items);
        }

        public static implicit operator CollectionChange((NotifyCollectionChangedAction , IEnumerable<object>) value)
        {
            return new CollectionChange(value.Item1, value.Item2);
        }
    }
}
