using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper.NonGeneric;
using UtilityInterface.Generic;
using UtilityInterface.NonGeneric;
using UtilityWpf.ViewModel;

namespace UtilityWpf.Interactive.Demo.Controls
{
    public class InteractiveList : ListBox
    {
        private readonly ObservableCollection<object> changeCollection = new ObservableCollection<object>();

        public readonly ISubject<KeyValuePair<IObject<object>, ChangeReason>> changes = new Subject<KeyValuePair<IObject<object>, ChangeReason>>();
        //protected ISubject<IEnumerable> ItemsSourceSubject = new Subject<IEnumerable>();
        protected ISubject<string> KeySubject = new ReplaySubject<string>();
        protected ISubject<string> GroupSubject = new ReplaySubject<string>();
        protected ISubject<Unit> ClearedSubject = new ReplaySubject<Unit>();

        protected ISubject<object> SelectedItemSubject = new ReplaySubject<object>();
        protected ISubject<object> DoubleClickedItemSubject = new ReplaySubject<object>();
        protected ISubject<object> DeletedSubject = new ReplaySubject<object>();
        protected ISubject<IFilter> FilterSubject = new ReplaySubject<IFilter>();
        protected ISubject<bool> IsRemovableSubject = new ReplaySubject<bool>();
        //protected ISubject<string> FilterOnSubject = new Subject<string>();
        protected ISubject<bool> IsReadOnlySubject = new ReplaySubject<bool>();
        protected ISubject<IEnumerable> DataSubject = new ReplaySubject<IEnumerable>();
        protected ISubject<bool> DoubleClickToCheckSubject = new ReplaySubject<bool>();

        private InteractiveCollectionBase<object> interactivecollectionBase;

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.KeySubject)));
        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register("Group", typeof(string), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.GroupSubject)));
        //   public static readonly DependencyProperty ClearedProperty = DependencyProperty.Register("Cleared", typeof(IFilter), typeof(ListBoxEx), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.SelectedItemSubject)));
        public static readonly DependencyProperty AllChangesProperty = DependencyProperty.Register("AllChanges", typeof(IEnumerable), typeof(InteractiveList), new PropertyMetadata(null));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(InteractiveList), new PropertyMetadata(Orientation.Horizontal));

        public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.SelectedItemSubject)));
        public static readonly DependencyProperty DoubleClickedItemProperty = DependencyProperty.Register("DoubleClickedItem", typeof(object), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.DoubleClickedItemSubject)));
        public static readonly DependencyProperty DeletedProperty = DependencyProperty.Register("Deleted", typeof(object), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.DeletedSubject)));
        public static readonly DependencyProperty IsRemovableProperty = DependencyProperty.Register("IsRemovable", typeof(bool), typeof(InteractiveList), new PropertyMetadata(true, (d, e) => Observe(d, e, a => a.IsRemovableSubject)));
        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(IEnumerable), typeof(InteractiveList), new PropertyMetadata(null));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(InteractiveList), new PropertyMetadata(false, (d, e) => Observe(d, e, a => a.IsReadOnlySubject)));
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IFilter), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.FilterSubject)));
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IEnumerable), typeof(InteractiveList), new PropertyMetadata(null, (d, e) => Observe(d, e, a => a.DataSubject)));
        public static readonly DependencyProperty DoubleClickToCheckProperty = DependencyProperty.Register("DoubleClickToCheck", typeof(bool), typeof(InteractiveList), new PropertyMetadata(false, (d, e) => Observe(d, e, a => a.DoubleClickToCheckSubject)));


        public override void OnApplyTemplate()
        {
            if (Key != null) this.KeySubject.OnNext(Key);
            if (Group != null) this.GroupSubject.OnNext(Key);
            //this.SelectedItemSubject.OnNext(this.SelectedItem);
            this.IsRemovableSubject.OnNext(IsRemovable);
            //this.KeySubject.OnNext(Key);
            if (null != Deleted) DeletedSubject.OnNext(Deleted);
            if (null != Filter) FilterSubject.OnNext(Filter);
            IsRemovableSubject.OnNext(IsRemovable);
            IsReadOnlySubject.OnNext(IsReadOnly);
            DoubleClickToCheckSubject.OnNext(DoubleClickToCheck);
            //DataSubject.OnNext(Data);
            base.OnApplyTemplate();
        }

        private static void Observe<T>(DependencyObject d, DependencyPropertyChangedEventArgs e, Func<InteractiveList, IObserver<T>> observer)
        {
            observer(d as InteractiveList).OnNext((T)e.NewValue);
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public IEnumerable AllChanges
        {
            get { return (IEnumerable)GetValue(AllChangesProperty); }
            set { SetValue(AllChangesProperty, value); }
        }

        public IEnumerable Checked
        {
            get { return (IEnumerable)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public bool IsRemovable
        {
            get { return (bool)GetValue(IsRemovableProperty); }
            set { SetValue(IsRemovableProperty, value); }
        }

        public IFilter Filter
        {
            get { return (IFilter)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public string Group
        {
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public object Deleted
        {
            get { return GetValue(DeletedProperty); }
            set { SetValue(DeletedProperty, value); }
        }

        public object DoubleClickedItem
        {
            get { return GetValue(DoubleClickedItemProperty); }
            set { SetValue(DoubleClickedItemProperty, value); }
        }

        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public bool DoubleClickToCheck
        {
            get { return (bool)GetValue(DoubleClickToCheckProperty); }
            set { SetValue(DoubleClickToCheckProperty, value); }
        }



        //public string GroupProperty
        //{
        //    get { return (string)GetValue(GroupPropertyProperty); }
        //    set { SetValue(GroupPropertyProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for GroupProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty GroupPropertyProperty =            DependencyProperty.Register("GroupProperty", typeof(string), typeof(InteractiveList), new PropertyMetadata(null));




        public IObservable<KeyValuePair<IObject<object>, ChangeReason>> Changes => changes.AsObservable();



        static InteractiveList()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(InteractiveList), new FrameworkPropertyMetadata(typeof(InteractiveList)));
            //ItemsSourceProperty.OverrideMetadata(typeof(InteractiveList), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ItemsSourceChanged, ItemsSourceCoerce));
        }

        public InteractiveList(Func<object, object> _keyfunc) : this()
        {
            interactivecollectionBase = InteractiveCollectionFactory.Build(
                getkeyObservable: Observable.Return(_keyfunc),
                  elems: DataSubject.Select(v => v.Cast<object>()),
                filter: FilterSubject,
                deletedObservable: DeletedSubject.Where(v => v != null),
                clearedObervable: ClearedSubject,
                deleteableObervable: IsRemovableSubject,
                isReadOnly: Observable.Return(IsReadOnly),
               doubleClickToCheck: DoubleClickToCheckSubject
              );
            CollectionChanged();
        }

        public InteractiveList(string key) : this()
        {
            Key = key;
        }

        public InteractiveList()
        {
            //CollectionChanged();
            Init();
        }

        private void Init()
        {
            var key = KeySubject.DistinctUntilChanged()
                .Merge(DataSubject
                .Select(_ => GetKey(_))
                .Where(_ => _ != null))
                .DistinctUntilChanged();

            Build(key)
                .Subscribe(a =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                            interactivecollectionBase = a;
                            CollectionChanged();
                    },
                    System.Windows.Threading.DispatcherPriority.Background, default);
                });

            //GroupSubject.Subscribe(a =>
            //{
                BuildGroup(key)
                    .Subscribe(a =>
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            interactivecollectionBase = a;
                            CollectionChanged();
                        },
                                    System.Windows.Threading.DispatcherPriority.Background, default);
                    });
            //});
        }


        private void CollectionChanged()
        {
            ItemsSource = interactivecollectionBase.Items;

            interactivecollectionBase.SelectDoubleClicked().Subscribe(_ =>
               Dispatcher.InvokeAsync(() => DoubleClickedItem = _, System.Windows.Threading.DispatcherPriority.Background, default));

            interactivecollectionBase.SelectSelected().Subscribe(_ =>
               Dispatcher.InvokeAsync(() => SelectedItem = _, System.Windows.Threading.DispatcherPriority.Background, default));

            interactivecollectionBase.SelectRemoved().Subscribe(_ =>
               Dispatcher.InvokeAsync(() => Deleted = _, System.Windows.Threading.DispatcherPriority.Background, default));

            interactivecollectionBase.Changes.Subscribe(_ => { changes.OnNext(_); changeCollection.Add(_); if (_.Value == ChangeReason.Add) ItemsSource = interactivecollectionBase.Items; });

            Dispatcher.InvokeAsync(() => AllChanges = changeCollection, System.Windows.Threading.DispatcherPriority.Background, default);

            Dispatcher.InvokeAsync(() => Checked = interactivecollectionBase.Checked, System.Windows.Threading.DispatcherPriority.Background, default);
        }

        private IObservable<InteractiveCollectionBase<object>> Build(IObservable<string> key)
        {
            return Observable.Return(InteractiveCollectionFactory.Build(
                       key.Select(_ => GetKeyFunc(_)),
                       elems: DataSubject.Select(v => v.Cast<object>()),
                       FilterSubject,
                       DeletedSubject.WithLatestFrom(IsRemovableSubject.DistinctUntilChanged(), (d, r) => r ? d : null).Where(v => v != null),
                       ClearedSubject,
                       deleteableObervable: IsRemovableSubject,
                       doubleClickToCheck: DoubleClickToCheckSubject
                       ));
        }

        private IObservable<InteractiveCollectionBase<object>> BuildGroup(IObservable<string> key)
        {
            //var UI = new DispatcherScheduler(Application.Current.Dispatcher);

            return Observable.Return(InteractiveCollectionFactory.BuildGroup(
                       key.Select(_ => GetKeyFunc(_)),
                       elems: DataSubject.Select(v => v.Cast<object>()),
                       FilterSubject,
                       DeletedSubject.WithLatestFrom(IsRemovableSubject.DistinctUntilChanged(), (d, r) => r ? d : null).Where(v => v != null),
                       ClearedSubject,
                       deleteableObervable: IsRemovableSubject,
                       doubleClickToCheck: DoubleClickToCheckSubject,
                       groupParameter: GroupSubject
                       ));
        }




        //public virtual object GetKey(object trade)
        //{
        //    //var type = trade.GetType().GetProperty(Key);
        //    //var interfaces =type .GetInterfaces();
        //    //if (!type.IsAssignableFrom(typeof(IConvertible)) && !interfaces.Select(_=>_.Name).Any(_=>_.StartsWith("IEquatable")))
        //    //    throw new Exception("Key of type "+ type.Name+ " does not inherit " + nameof(IConvertible) + " or "+ "IEquatable");
        //    //else
        //    return null;

        //}

        public virtual string GetKey(IEnumerable _)
        {
            if (_.Count() > 0)
            {
                var type = _?.First()?.GetType();
                if (type != null)
                {
                    var xx = UtilityHelper.IdHelper.GetIdProperty(type);
                    if (xx != null)
                    {
                        var sxx = UtilityHelper.IdHelper.CheckIdProperty(xx, type);
                        if (sxx)
                            return xx;
                        else
                            return null;
                    }
                }
            }
            return null;
        }

        private Func<object, object> GetKeyFunc(string key)
        {
            object f(object o)
            {
                //var @object = UtilityHelper.PropertyHelper.GetPropValue<object>(o, "Object");
                return UtilityHelper.PropertyHelper.GetPropertyValue<object>(o, key);
            }
            return f;
        }
    }
}


