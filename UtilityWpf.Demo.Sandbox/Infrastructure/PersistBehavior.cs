using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DynamicData.Binding;

namespace UtilityDAL.View.Behavior {
   public class PersistBehavior : Behavior<ItemsControl> {
      private IDisposable disposable;
      private readonly ReplaySubject<IDatabaseService> changes = new(1);

      public static readonly DependencyProperty RepositoryProperty = DependencyProperty.Register("Repository", typeof(IDatabaseService), typeof(PersistBehavior), new PropertyMetadata(null, RepositoryChanged));
      private ReadOnlyObservableCollection<object> items;

      #region properties
      public IDatabaseService Repository {
         get { return (IDatabaseService)GetValue(RepositoryProperty); }
         set { SetValue(RepositoryProperty, value); }
      }
      #endregion properties

      public IReadOnlyCollection<object> Items => items;

        protected override void OnAttached() {
         base.OnAttached();

         var changeSet = ObservableChangeSet.Create<object>(observer => {
            return changes
               .Select(a => a.SelectAll().Cast<object>())
               .Subscribe(a => {
                  observer.Clear();
                  observer.AddRange(a);
               });
         });

         changeSet
            .Bind(out items)
            .Subscribe();

         changes.OnNext(Repository);

         if (AssociatedObject.ItemsSource is INotifyCollectionChanged collectionChanged) {
            var b = collectionChanged
                .SelectChanges()
                .SelectMany(a => (a.OldItems?.Cast<object>() ?? Array.Empty<object>())
                .Select(av => (a.Action, av)));

            var a = collectionChanged
                .SelectChanges()
                .SelectMany(a => (a.NewItems?.Cast<object>() ?? Array.Empty<object>()).Select(av => (a.Action, av)));

            disposable = a.Merge(b)
                .WithLatestFrom(changes.WhereNotNull())
                .Subscribe(cc => {
                   var ((reason, item), docstore) = cc;
                   if (reason == NotifyCollectionChangedAction.Add) {
                      docstore.Insert(item);
                   }
                   else if (reason == NotifyCollectionChangedAction.Remove) {
                      docstore.Delete(item);
                   }
                       //else if (reason == NotifyCollectionChangedAction.)
                       //{
                       //    _docstore.Upsert(item);
                       //}
                    });
            return;
         }
      }

      protected override void OnDetaching() {
         base.OnDetaching();
         disposable?.Dispose();
      }

      private static void RepositoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
         (dependencyObject as PersistBehavior).changes.OnNext(e.NewValue as IDatabaseService);
      }
   }
}
