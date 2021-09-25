//using System;
//using System.Collections.Generic;
//using System.Windows;
//using Microsoft.Xaml.Behaviors;
//using System.Windows.Controls;
//using System.Collections.Specialized;
//using UtilityInterface.NonGeneric.Data;
//using System.Reactive.Subjects;
//using System.Windows.Input;
//using System.ComponentModel;
//using System.Reactive.Linq;
//using Utility.Common;

//namespace UtilityWpf.Behavior
//{
//    public class PersistBehavior : Behavior<ItemsControl>
//    {
//        private IDisposable? disposable;
//        private readonly ReplaySubject<IRepository> repositoryChanges = new(1);
//        private readonly PersistService persistService = new();

//        public static readonly DependencyProperty RepositoryProperty = DependencyProperty.Register("Repository", typeof(IRepository), typeof(PersistBehavior), new PropertyMetadata(null, RepositoryChanged));
//        public static readonly DependencyProperty CollectionChangeCommandProperty = DependencyProperty.Register("CollectionChangeCommand", typeof(ICommand), typeof(PersistBehavior), new PropertyMetadata(null));

//        #region properties
//        public IRepository Repository
//        {
//            get { return (IRepository)GetValue(RepositoryProperty); }
//            set { SetValue(RepositoryProperty, value); }
//        }


//        public ICommand CollectionChangeCommand
//        {
//            get { return (ICommand)GetValue(CollectionChangeCommandProperty); }
//            set { SetValue(CollectionChangeCommandProperty, value); }
//        }
//        #endregion properties

//        protected override void OnAttached()
//        {
//            base.OnAttached();

//            repositoryChanges.Select(a => new RepositoryMessage(a)).Subscribe(persistService);
//            disposable = persistService.Select(a => a).Subscribe(a =>
//            { 
//                 CollectionChangeCommand?.Execute(a.Objects);
//            });
//            AssociatedObject.ItemsSource = persistService.Items;
//        }

//        private void Change_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void OnDetaching()
//        {
//            base.OnDetaching();
//            disposable?.Dispose();
//        }

//        private static void RepositoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
//        {
//            (dependencyObject as PersistBehavior).repositoryChanges.OnNext(e.NewValue as IRepository);
//        }

//        //protected void RaiseCollectionChangedEvent(string text)
//        //{
//        //    this.RaiseEvent(new TextRoutedEventArgs(CollectionChangedEvent, text));
//        //}
//    }

  
//}
