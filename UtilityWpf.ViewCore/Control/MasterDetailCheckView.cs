using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.ViewModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Input;
using PropertyTools.Wpf;
using System.Collections.ObjectModel;
using UtilityInterface.NonGeneric;
using DynamicData;

namespace UtilityWpf.View
{
    public class MasterDetailCheckView : Controlx
    {
        public ICommand GroupClick { get; }


        #region properties

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public Control DetailView
        {
            get { return (Control)GetValue(DetailViewProperty); }
            set { SetValue(DetailViewProperty, value); }
        }

        public PropertyGroupDescription PropertyGroupDescription
        {
            get { return (PropertyGroupDescription)GetValue(PropertyGroupDescriptionProperty); }
            set { SetValue(PropertyGroupDescriptionProperty, value); }
        }

        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }


        #endregion properties


        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(MasterDetailCheckView), new PropertyMetadata("Id", Changed));

        public static readonly DependencyProperty DetailViewProperty = DependencyProperty.Register("DetailView", typeof(Control), typeof(MasterDetailCheckView), new PropertyMetadata(null, DetailViewChanged));

        public static readonly DependencyProperty PropertyGroupDescriptionProperty = DependencyProperty.Register("PropertyGroupDescription", typeof(PropertyGroupDescription), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register("DataConverter", typeof(IValueConverter), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        private static void DetailViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MasterDetailCheckView).ControlChanges.OnNext(e.NewValue as Control);
        }

        protected ISubject<string> GroupNameChanges = new Subject<string>();
        protected ISubject<string> NameChanges = new Subject<string>();
        private ReadOnlyObservableCollection<object> collection;


        public ICollection<UtilityInterface.Generic.IContain<object>> Objects { get; }

        static MasterDetailCheckView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailCheckView), new FrameworkPropertyMetadata(typeof(MasterDetailCheckView)));
        }

        public MasterDetailCheckView()
        {
            ISubject<IObservable<object>> subjectObjects = new Subject<IObservable<object>>();

            var ic = InteractiveCollectionFactory.Build(subjectObjects.Switch(), Observable.Return(new DefaultFilter()), Observable.Empty<object>());

            Objects = ic.Items;

            this.Dispatcher.Invoke(() =>
            {
                //(its.a.Resources["GroupedItems"] as CollectionViewSource).Source = ic.Items;

            });

            SelectControlChanges<DockPanel>().CombineLatest(SelectChanges(nameof(Items)), (a, b) => (a, b))
                .Subscribe(its =>
                    {
                        subjectObjects.OnNext((its.b as IEnumerable).MakeObservable());
                    });

            var xx = SelectChanges<string>(nameof(MasterDetailView.Id)).StartWith(Id)
                 .CombineLatest(subjectObjects.Switch(), (a, b) => (a, b))
                 .Select(a =>
             {
                 var prop = a.b.GetType().GetProperty(a.a.ToString());
                 return ic.SelectCheckedChanges().ToObservableChangeSet(c => prop.GetValue(c.Item2));
             })
                 .Switch()
                 .Filter(a => a.Item1)
             .Cast(a => a.Item2)
             .Bind(out collection)
             .DisposeMany()
             .Subscribe(a =>
             {

             });

            //this.Loaded += MasterDetailCheckView_Loaded;

            SelectChanges<Control>(nameof(MasterDetailCheckView.DetailView)).StartWith(DetailView)
                .Merge(this.SelectLoads().Select(a => this.DetailView))

                 .DistinctUntilChanged(a => a?.GetType().Name).Subscribe(detailView =>
              {
                  detailView = detailView ?? new PropertyGrid();

                  if (detailView is Abstract.IItemsSource oview)
                  {
                      oview.ItemsSource = collection;
                  }
                  else if (detailView is PropertyGrid propertyGrid)
                  {
                      propertyGrid.SelectedObjects = collection;

                  }
                  else if (detailView is ItemsControl itemsControl)
                  {
                      itemsControl.ItemsSource = collection;
                  }
                  else
                      throw new Exception(nameof(DetailView) + " needs to have property OutputView");


              });
        }


        class DefaultFilter : UtilityInterface.NonGeneric.IFilter
        {
            public bool Filter(object o)
            {
                return true;
            }
        }



        public class KeyValue
        {
            public KeyValue(string key, object value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; }

            public object Value { get; }
        }
    }
}