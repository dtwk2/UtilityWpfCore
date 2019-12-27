using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper.NonGeneric;
using UtilityInterface.Generic;
using UtilityInterface.NonGeneric;
using UtilityWpf.ViewModel;

namespace UtilityWpf.View
{
    public class ListBoxEx : ListBox
    {
        public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(ListBoxEx), new PropertyMetadata(null, SelectedItemChanged));

        public static readonly DependencyProperty DoubleClickedItemProperty = DependencyProperty.Register("DoubleClickedItem", typeof(object), typeof(ListBoxEx), new PropertyMetadata(null, DoubleClickedItemChanged));

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(ListBoxEx), new PropertyMetadata(null, KeyChanged));

        public static readonly DependencyProperty DeletedProperty = DependencyProperty.Register("Deleted", typeof(object), typeof(ListBoxEx), new PropertyMetadata(null, DeletedChanged));

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IFilter), typeof(ListBoxEx), new PropertyMetadata(null, FilteredChanged));

        public static readonly DependencyProperty RemoveProperty = DependencyProperty.Register("Remove", typeof(bool), typeof(ListBoxEx), new PropertyMetadata(true, RemoveChanged));

        //public static readonly DependencyProperty FilterOnProperty = DependencyProperty.Register("FilterOn", typeof(string), typeof(ListBoxEx), new PropertyMetadata(null, FilterOnChanged));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ListBoxEx), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty AllChangesProperty = DependencyProperty.Register("AllChanges", typeof(IEnumerable), typeof(ListBoxEx), new PropertyMetadata(null));

        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(IEnumerable), typeof(ListBoxEx), new PropertyMetadata(null));

        //public static readonly DependencyProperty ItemsSourceExProperty = DependencyProperty.Register("ItemsSourceEx", typeof(IEnumerable), typeof(ListBoxEx), new PropertyMetadata(null, null, ItemsSourceExChanged));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ListBoxEx), new PropertyMetadata(false, IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListBoxEx).IsReadOnlyChanges.OnNext((bool)e.NewValue);
        }

        //public IEnumerable ItemsSourceEx
        //{
        //    get { return (IEnumerable)GetValue(ItemsSourceExProperty); }
        //    set { SetValue(ItemsSourceExProperty, value); }
        //}

        public IEnumerable AllChanges
        {
            get { return (IEnumerable)GetValue(AllChangesProperty); }
            set { SetValue(AllChangesProperty, value); }
        }

        //public IEnumerable ItemsSource
        //{
        //    get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        //    set { SetValue(ItemsSourceProperty, value); }
        //}

        //public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ListBoxEx), new PropertyMetadata(null, ItemsSourceChanged));

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

        public bool Remove
        {
            get { return (bool)GetValue(RemoveProperty); }
            set { SetValue(RemoveProperty, value); }
        }

        public IFilter Filter
        {
            get { return (IFilter)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        //public string FilterOn
        //{
        //    get { return (string)GetValue(FilterOnProperty); }
        //    set { SetValue(FilterOnProperty, value); }
        //}

        // Using a DependencyProperty as the backing store for FilterProperty.  This enables animation, styling, binding, etc...

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public new object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public object Deleted
        {
            get { return (object)GetValue(DeletedProperty); }
            set { SetValue(DeletedProperty, value); }
        }

        public object DoubleClickedItem
        {
            get { return (object)GetValue(DoubleClickedItemProperty); }
            set { SetValue(DoubleClickedItemProperty, value); }
        }

        private static void DoubleClickedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void DeletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListBoxEx).DeletedSubject.OnNext((object)e.NewValue);
        }

        private static void KeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListBoxEx).KeySubject.OnNext((string)e.NewValue);
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object ItemsSourceCoerce(DependencyObject d, object baseValue)
        {
            if (baseValue != null && ((IEnumerable<object>)baseValue).Count() > 0)
            {
                var yd = ((IEnumerable<object>)baseValue)?.Select(_ => new { a = _, b = _.GetType().GetProperties().SingleOrDefault(__ => __.Name == "Object") }).ToArray();

                var tyy2 = yd?.Where(_ => _.b != null).Select(_ => _.b.GetValue(_.a));
                var tyy = yd?.Where(_ => _.b == null).Select(_ => _.a).Concat(tyy2).ToArray();

                if (tyy != null && tyy.Count() > 0)
                {
                    (d as ListBoxEx).ItemsSourceSubject.OnNext(tyy);
                    return ((IEnumerable<object>)baseValue).Where(_ => !tyy.Contains(_)).ToArray();
                }
                else
                    return baseValue;
            }
            else
                return baseValue;
        }

        private static void FilteredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListBoxEx).FilterSubject.OnNext((IFilter)e.NewValue);
        }

        private static void RemoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListBoxEx).RemoveSubject.OnNext((bool)e.NewValue);
        }

        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListBoxEx).SelectedItemSubject.OnNext(((object)e.NewValue));
        }

        protected ISubject<bool> IsReadOnlyChanges = new Subject<bool>();
        protected ISubject<object> SelectedItemSubject = new Subject<object>();
        protected ISubject<IEnumerable> ItemsSourceSubject = new Subject<IEnumerable>();
        protected ISubject<string> KeySubject = new Subject<string>();
        protected ISubject<object> DeletedSubject = new Subject<object>();
        protected ISubject<object> ClearedSubject = new Subject<object>();
        protected ISubject<IFilter> FilterSubject = new Subject<IFilter>();
        protected ISubject<bool> RemoveSubject = new Subject<bool>();
        //protected ISubject<string> FilterOnSubject = new Subject<string>();

        private ViewModel.InteractiveCollectionViewModel<object, object> interactivecollection; /*{ get;  set; }*/

        public IObservable<KeyValuePair<IContain<Object>, ChangeReason>> Changes { get; private set; } = new Subject<KeyValuePair<IContain<Object>, ChangeReason>>();


        static ListBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxEx), new FrameworkPropertyMetadata(typeof(ListBoxEx)));
            ListBoxEx.ItemsSourceProperty.OverrideMetadata(typeof(ListBoxEx), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ItemsSourceChanged, ItemsSourceCoerce));
         
        }

        public ListBoxEx(Func<object, object> _keyfunc)
        {

        
            interactivecollection = ViewModel.InteractiveCollectionFactory.Build(
               _ => _keyfunc(_),
               ItemsSourceSubject.Select(v => v.Cast<object>()),
               FilterSubject,
               DeletedSubject.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => r ? d : null).Where(v => v != null),
               ClearedSubject,
               null, IsReadOnly
            );
            CollectionChanged();

            Init();
        }

        public ListBoxEx(string key)
        {


         
            if (key != null)
            {
                Key = key;
                interactivecollection = ViewModel.InteractiveCollectionFactory.Build(
                   GetKeyFunc(key),
                   ItemsSourceSubject.Select(v => v.Cast<object>()),
                   FilterSubject,
                   DeletedSubject.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => r ? d : null).Where(v => v != null),
                   ClearedSubject,
                   null, IsReadOnly
                );
                CollectionChanged();
            }

            Init();
        }

        public ListBoxEx()
        {

            if (Key != null)
            {
                interactivecollection = ViewModel.InteractiveCollectionFactory.Build(
                   GetKeyFunc(Key),
                   ItemsSourceSubject.Select(v => v.Cast<object>()),
                   FilterSubject,
                   DeletedSubject.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => r ? d : null).Where(v => v != null),
                   ClearedSubject,
                   null, IsReadOnly
                );
                CollectionChanged();
            }

            Init();
        }

        private void Init()
        {
            var key = KeySubject.DistinctUntilChanged().Merge(ItemsSourceSubject.Take(1)
    .Select(_ => GetKey(_))
    .Where(_ => _ != null)).DistinctUntilChanged();

            Build(ItemsSourceSubject.Select(v => v.Cast<object>()), key).Subscribe(a_ =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    interactivecollection = a_;
                    CollectionChanged();
                },
                System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            });
        }

        private ObservableCollection<object> changeCollection = new ObservableCollection<object>();

        private IObservable<InteractiveCollectionViewModel<object, object>> Build(IObservable<IEnumerable<object>> observable, IObservable<string> key)
        {
            var UI = new System.Reactive.Concurrency.DispatcherScheduler(Application.Current.Dispatcher);

            return ViewModel.InteractiveCollectionFactory.Build(
                       key.Select(_ => GetKeyFunc(_)),
                       observable,
                       FilterSubject,
                       DeletedSubject.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => r ? d : null).Where(v => v != null),
                       ClearedSubject,
                       UI);
        }

        private void CollectionChanged()
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                ItemsSource = interactivecollection.Items;
            }, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));

            interactivecollection.GetDoubleClicked().Subscribe(_ =>
               this.Dispatcher.InvokeAsync(() => DoubleClickedItem = _, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken)));

            interactivecollection.GetSelected().Subscribe(_ =>
               this.Dispatcher.InvokeAsync(() => SelectedItem = _, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken)));

            interactivecollection.GetRemoved().Subscribe(_ =>
               this.Dispatcher.InvokeAsync(() => Deleted = _, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken)));

            interactivecollection.Changes.Subscribe(_ => { (Changes as ISubject<KeyValuePair<IContain<Object>, ChangeReason>>).OnNext(_); changeCollection.Add(_); if (_.Value == ChangeReason.Add) ItemsSource = interactivecollection.Items; });

            this.Dispatcher.InvokeAsync(() => AllChanges = changeCollection, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));

            this.Dispatcher.InvokeAsync(() => Checked = interactivecollection.@checked, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
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
            Func<object, object> f = o =>
            {
                //var @object = UtilityHelper.PropertyHelper.GetPropValue<object>(o, "Object");
                return UtilityHelper.PropertyHelper.GetPropertyValue<object>(o, key);
            };
            return f;
        }
    }
}