using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Collections.Specialized;
using UtilityHelperEx;
using System.Reactive.Linq;
using UtilityInterface.NonGeneric.Database;
using System.Reactive.Subjects;
using DynamicData;
using System.Windows.Input;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace UtilityWpf.Behavior
{
    public class PersistBehavior : Behavior<ItemsControl>
    {
        private IDisposable? disposable;
        private readonly ReplaySubject<IDatabaseService> changes = new(1);

        public static readonly DependencyProperty RepositoryProperty = DependencyProperty.Register("Repository", typeof(IDatabaseService), typeof(PersistBehavior), new PropertyMetadata(null, RepositoryChanged));
        public static readonly DependencyProperty CollectionChangeCommandProperty = DependencyProperty.Register("CollectionChangeCommand", typeof(ICommand), typeof(PersistBehavior), new PropertyMetadata(null));

        #region properties
        public IDatabaseService Repository
        {
            get { return (IDatabaseService)GetValue(RepositoryProperty); }
            set { SetValue(RepositoryProperty, value); }
        }


        public ICommand CollectionChangeCommand
        {
            get { return (ICommand)GetValue(CollectionChangeCommandProperty); }
            set { SetValue(CollectionChangeCommandProperty, value); }
        }
        #endregion properties
        public MintPlayer.ObservableCollection.ObservableCollection<object> Items { get; } = new();

        protected override void OnAttached()
        {
            base.OnAttached();

            var compositeDisposable = new CompositeDisposable();

            IObservable<CollectionChange> a = Items
                .SelectChanges()
                .Select(a => new CollectionChange(a.Action, a.NewItems?.Cast<object>()?.ToArray() ?? Array.Empty<object>()));

            var b = Items
                .SelectChanges()
                .Select(a => new CollectionChange(a.Action, a.OldItems?.Cast<object>()?.ToArray() ?? Array.Empty<object>()));

            a.Merge(b)
             .Scan(default((CollectionChange, CollectionChange)), (a, b) => (a.Item2, b))
             .WithLatestFrom(changes.WhereNotDefault())
             .Subscribe(cc =>
             {
                 var (((action, first), (second, third)), docstore) = cc;
                 if (action == NotifyCollectionChangedAction.Reset)
                     return;


                 if (second == NotifyCollectionChangedAction.Replace)
                 {
                     foreach (var item in third)
                         docstore.Update(item);
                 }
                 if (second == NotifyCollectionChangedAction.Add)
                 {
                     foreach (var item in third)
                         docstore.Insert(item);
                 }
                 else if (second == NotifyCollectionChangedAction.Remove)
                 {
                     foreach (var item in third)
                         docstore.Delete(item);
                 }
                 //else if (reason == NotifyCollectionChangedAction.)
                 //{
                 //    _docstore.Upsert(item);
                 //}
             }).DisposeWith(compositeDisposable);

            var changeSet = ObservableChangeSet.Create<object, string>(observer =>
             {
                 return changes
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
                        CollectionChangeCommand?.Execute(observer.Items);


                        Items.Clear();
                        Items.AddRange(objects);
                    });


             }, a => a.ToString());


            changeSet
                .OnItemUpdated((a, sd) =>
                {

                    Items.Replace(a, sd);
                })
                .Subscribe(a =>
                {

                }).DisposeWith(compositeDisposable);

            AssociatedObject.ItemsSource = Items;

            changes.Subscribe(a =>
            {

            }).DisposeWith(compositeDisposable);

            disposable = compositeDisposable;            
            // }
        }

        private void Change_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            disposable?.Dispose();
        }

        private static void RepositoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as PersistBehavior).changes.OnNext(e.NewValue as IDatabaseService);
        }

        //protected void RaiseCollectionChangedEvent(string text)
        //{
        //    this.RaiseEvent(new TextRoutedEventArgs(CollectionChangedEvent, text));
        //}
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
