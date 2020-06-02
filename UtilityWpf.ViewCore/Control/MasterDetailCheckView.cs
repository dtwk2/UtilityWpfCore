using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.ViewModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using UtilityWpf.Abstract;

namespace UtilityWpf.View
{
    public class MasterDetailCheckView : ContentControlx
    {
        protected ISubject<string> GroupNameChanges = new Subject<string>();
        protected ISubject<string> NameChanges = new Subject<string>();
        private ReadOnlyObservableCollection<object> collection;
        public ICommand GroupClick { get; }

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(MasterDetailCheckView), new PropertyMetadata("Id", Changed));

        public static readonly DependencyProperty DetailViewProperty = DependencyProperty.Register("DetailView", typeof(Control), typeof(MasterDetailCheckView), new PropertyMetadata(null, (d, e) => (d as MasterDetailCheckView).ControlChanges.OnNext(e.NewValue as Control)));

        public static readonly DependencyProperty PropertyGroupDescriptionProperty = DependencyProperty.Register("PropertyGroupDescription", typeof(PropertyGroupDescription), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register("DataConverter", typeof(IValueConverter), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));


        #region properties

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
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



        public ICollection<UtilityInterface.Generic.IContain<object>> Objects { get; }

        static MasterDetailCheckView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailCheckView), new FrameworkPropertyMetadata(typeof(MasterDetailCheckView)));
        }

        public MasterDetailCheckView()
        {
            //ISubject<IObservable<object>> subjectObjects = new Subject<IObservable<object>>();

            var objects = SelectChanges(nameof(Items))
                 .Select(its => (its as IEnumerable).MakeObservable());

            var ic = InteractiveCollectionFactory.Build(objects.Switch(), Observable.Return(new DefaultFilter()), Observable.Empty<object>());

            Objects = ic.Items;

            var one = this.WhenAnyValue(c => c.Content).Merge(this.SelectLoads().Select(a => this.Content ??= new JsonView()))
                .DistinctUntilChanged(a => a?.GetType().Name);


            var xx = SelectChanges<string>(nameof(MasterDetailView.Id))
                .StartWith(Id)
                 .CombineLatest(objects.Switch(), (id, b) => b.GetType().GetProperty(id))
                 .Select(prop => ic.SelectCheckedChanges().ToObservableChangeSet(c => prop.GetValue(c.Item2)))
                 .Switch()
                 .Filter(a => a.isChecked)
                 .Transform(a => a.obj)
                 .ToCollection()
                .CombineLatest(one, (collection, obj) => (collection, obj))
                .Subscribe(ab =>
                {
                    SetCollection(ab.obj, ab.collection);
                });

        }


        protected virtual void SetCollection(object content, IReadOnlyCollection<object> objects)
        {
            if (content is IItemsSource oview)
            {
                oview.ItemsSource = objects;
            }
            else if (content is JsonView propertyGrid)
            {
                propertyGrid.Object = objects;
            }
            else if (content is ItemsControl itemsControl)
            {
                itemsControl.ItemsSource = objects;
            }
            else throw new Exception(nameof(Content) + " needs to have property");
        }

        class DefaultFilter : UtilityInterface.NonGeneric.IFilter
        {
            public bool Filter(object o)
            {
                return true;
            }
        }


    }
}