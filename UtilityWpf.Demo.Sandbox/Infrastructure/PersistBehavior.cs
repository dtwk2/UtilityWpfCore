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
using ReactiveUI;

namespace UtilityDAL.View.Behavior
{
    public class PersistBehavior : Behavior<ItemsControl>
    {
        private IDisposable disposable;
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

        //  public ReadOnlyObservableCollection<object> Items => items;

        protected override void OnAttached()
        {
            base.OnAttached();


            IObservable<NewStruct> a = Items
                .SelectChanges()
                .Select(a => new NewStruct(a.Action, a.NewItems?.Cast<object>() ?? Array.Empty<object>()));

            var b = Items
                .SelectChanges()
                .Select(a => new NewStruct(a.Action, a.OldItems?.Cast<object>() ?? Array.Empty<object>()));

            disposable = a.Merge(b)
             .Scan(default((NewStruct, NewStruct)), (a, b) => (a.Item2, b))
             .WithLatestFrom(changes.WhereNotDefault())
             .Subscribe(cc =>
             {
                 var ((first, (second, third)), docstore) = cc;
                 if (first.Action == NotifyCollectionChangedAction.Reset)
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
             });

            var changeSet = ObservableChangeSet.Create<object>(observer =>
            {
                return changes
                   .Select(a => a.SelectAll().Cast<object>())
                   .Subscribe(objects =>
                   {
                       foreach (var change in objects.OfType<INotifyPropertyChanged>())
                       {
                           change.WhenAnyValue(a => a)
                           .Subscribe(x =>
                           {
                               observer.Replace(x,x);
                           });
                       }
                       observer.Clear();
                       observer.AddRange(objects);
                       CollectionChangeCommand?.Execute(observer.Items);
                   });
            });

            changeSet
               .ToCollection()
               .Subscribe(a =>
               {
                   Items.Clear();
                   Items.AddRange(a);
               });

            AssociatedObject.ItemsSource = Items;
            //changes.OnNext(Repository);

            //if(AssociatedObject.ItemsSource==null)
            //{
            //    AssociatedObject.ItemsSource = new ObservableCollection<object>();
            //}
            //if (AssociatedObject.ItemsSource is INotifyCollectionChanged collectionChanged)
            //{

            changes.Subscribe(a =>
            {

            });


            return;
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

    internal struct NewStruct
    {
        public NotifyCollectionChangedAction Action;
        public IEnumerable<object> Item2;

        public NewStruct(NotifyCollectionChangedAction action, IEnumerable<object> item2)
        {
            Action = action;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return obj is NewStruct other &&
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

        public static implicit operator (NotifyCollectionChangedAction Action, IEnumerable<object>)(NewStruct value)
        {
            return (value.Action, value.Item2);
        }

        public static implicit operator NewStruct((NotifyCollectionChangedAction Action, IEnumerable<object>) value)
        {
            return new NewStruct(value.Action, value.Item2);
        }
    }





    //private static void CollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    (d as PathBrowser ?? throw new NullReferenceException("PathBrowser is null")).textBoxContentChanges.OnNext(e.NewValue);
    //}

    //public class CollectionChangedRoutedEventArgs : RoutedEventArgs
    //{
    //    public CollectionChangedRoutedEventArgs(RoutedEvent routedEvent, string text) : base(routedEvent)
    //    {
    //        Text = text;
    //    }

    //    public string Text { get; set; }
    //}

    //public delegate void TextChangeRoutedEventHandler(object sender, CollectionChanged e);
}
