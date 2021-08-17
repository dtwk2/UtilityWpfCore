//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Reactive.Subjects;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using System.Windows;

//namespace UtilityWpf.Demo.Sandbox.Infrastructure
//{
//    public class PersistBehavior
//    {


//    }
//}
using System;
using System.Linq;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Collections.Specialized;
using UtilityHelperEx;
using System.Reactive.Linq;
using UtilityInterface.NonGeneric.Database;
using System.Reactive.Subjects;

namespace UtilityDAL.View.Behavior
{
    public class PersistBehavior : Behavior<ItemsControl>
    {
        private IDisposable disposable;
        private readonly ReplaySubject<IDatabaseService> changes = new(1);

        public static readonly DependencyProperty RepositoryProperty = DependencyProperty.Register("Repository", typeof(IDatabaseService), typeof(PersistBehavior), new PropertyMetadata(null, RepositoryChanged));


 

        #region properties
        public IDatabaseService Repository
        {
            get { return (IDatabaseService)GetValue(RepositoryProperty); }
            set { SetValue(RepositoryProperty, value); }
        }
        #endregion properties


        protected override void OnAttached()
        {
            base.OnAttached();

            changes.OnNext(Repository);

            if (AssociatedObject.ItemsSource is INotifyCollectionChanged collectionChanged)
            {
                var b = collectionChanged
                    .SelectChanges()
                    .SelectMany(a => (a.OldItems?.Cast<object>()?? Array.Empty<object>())
                    .Select(av => (a.Action, av)));

                var a = collectionChanged
                    .SelectChanges()
                    .SelectMany(a => (a.NewItems?.Cast<object>() ?? Array.Empty<object>()).Select(av => (a.Action, av)));

                disposable = a.Merge(b)
                    .CombineLatest(changes.WhereNotNull())
                    .Subscribe(cc =>
                    {
                        var ((reason, item), _docstore) = cc;
                        if (reason == NotifyCollectionChangedAction.Add)
                        {
                            _docstore.Insert(item);
                        }
                        else if (reason == NotifyCollectionChangedAction.Remove)
                        {
                            _docstore.Delete(item);
                        }
                        //else if (reason == NotifyCollectionChangedAction.)
                        //{
                        //    _docstore.Upsert(item);
                        //}
                    });
                return;
            }
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
    }
}
