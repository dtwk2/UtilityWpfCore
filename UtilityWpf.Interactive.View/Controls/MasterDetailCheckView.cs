using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UtilityWpf.Abstract;
using UtilityWpf.View;

namespace UtilityWpf.Interactive.View.Controls
{
    public class MasterDetailCheckView : ContentControlx
    {
        protected ISubject<string> GroupNameChanges = new Subject<string>();
        protected ISubject<string> NameChanges = new Subject<string>();
        private InteractiveList interactiveList;

        //private ReadOnlyObservableCollection<object> collection;
        public ICommand GroupClick { get; }

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(MasterDetailCheckView), new PropertyMetadata("Id", Changed));

        public static readonly DependencyProperty DetailViewProperty = DependencyProperty.Register("DetailView", typeof(Control), typeof(MasterDetailCheckView), new PropertyMetadata(null, (d, e) => (d as MasterDetailCheckView).ControlChanges.OnNext(e.NewValue as Control)));

        public static readonly DependencyProperty PropertyGroupDescriptionProperty = DependencyProperty.Register("PropertyGroupDescription", typeof(PropertyGroupDescription), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MasterDetailCheckView), new PropertyMetadata(null, Changed));

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

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        #endregion properties

        //public ICollection<UtilityInterface.Generic.IObject<object>> Objects { get; }

        static MasterDetailCheckView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailCheckView), new FrameworkPropertyMetadata(typeof(MasterDetailCheckView)));
        }

        public MasterDetailCheckView()
        {
        }

        public override void OnApplyTemplate()
        {
            Content ??= new ItemsControl();

            interactiveList = this.GetTemplateChild("Main_InteractiveList") as InteractiveList;

            var objects = (this).SelectChanges<IEnumerable>(nameof(ItemsSource))
                .Select(its => its.MakeObservable())
                .Switch();

            var ic = InteractiveCollectionFactory.Build(objects, getkeyObservable: Observable.Return(new Func<object, object>(a => Guid.NewGuid())), filter: Observable.Return(new DefaultFilter()), isCheckableObervable: Observable.Return(true));
            //objects.Switch().Subscribe(ic => Objects = ic);

            interactiveList.InteractiveCollectionBase = ic.collectionViewModel;

            ic.collectionViewModel.Items.MakeObservable().Subscribe(a =>
           {
           });

            //.Merge(this.SelectLoads()
            ////.Select(a => Content ??= new JsonView()))
            //.DistinctUntilChanged(a => a?.GetType().Name));

            var xx = SelectChanges<string>(nameof(MasterDetailCheckView.Id))
                .StartWith(Id)
                 .CombineLatest(objects, (id, b) => b.GetType().GetProperty(id))
                 .Select(prop => ic.collectionViewModel.SelectCheckedChanges().ToObservableChangeSet(c => prop.GetValue(c.obj)))
                 .Switch()
                 .Filter(a => a.isChecked)
                 .Transform(a => a.obj)
                 .ToCollection()
                .CombineLatest(this.WhenAnyValue(c => c.Content)
                .Where(a => a != null), (collection, obj) => (collection, obj))
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

        private class DefaultFilter : UtilityInterface.NonGeneric.IFilter
        {
            public bool Filter(object o)
            {
                return true;
            }
        }
    }
}