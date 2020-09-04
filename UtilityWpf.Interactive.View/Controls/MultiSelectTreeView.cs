using DynamicData;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper.NonGeneric;
using UtilityWpf.View.Infrastructure;
using UtilityWpf.Interactive;
using UtilityWpf.Interactive.Model;

namespace UtilityWpf.Interactive.View.Controls
{
    //[ContentProperty("Items")]
    public class MultiSelectTreeView : TreeView
    {
        private static readonly string Children = nameof(Children);

        public static readonly DependencyProperty CheckedItemsProperty = DependencyProperty.Register("CheckedItems", typeof(IEnumerable), typeof(MultiSelectTreeView), new PropertyMetadata(null));

        public static readonly DependencyProperty AllCheckedItemsProperty = DependencyProperty.Register("AllCheckedItems", typeof(IEnumerable), typeof(MultiSelectTreeView), new PropertyMetadata(null));

        public static readonly DependencyProperty ChildrenPathProperty = DependencyProperty.Register(nameof(ChildrenPath), typeof(string), typeof(MultiSelectTreeView), new PropertyMetadata(Children, ChildrenPathChange));

        public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(MultiSelectTreeView));

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(MultiSelectTreeView), new PropertyMetadata("Key", KeyChanged));

        public static readonly DependencyProperty ExpandProperty = DependencyProperty.Register("Expand", typeof(bool), typeof(MultiSelectTreeView), new PropertyMetadata(true, ExpandChanged));

        public static readonly DependencyProperty CheckProperty = DependencyProperty.Register("Check", typeof(bool), typeof(MultiSelectTreeView), new PropertyMetadata(true, CheckChanged));

        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public IEnumerable CheckedItems
        {
            get { return (IEnumerable)GetValue(CheckedItemsProperty); }
            set { SetValue(CheckedItemsProperty, value); }
        }

        public IEnumerable AllCheckedItems
        {
            get { return (IEnumerable)GetValue(AllCheckedItemsProperty); }
            set { SetValue(AllCheckedItemsProperty, value); }
        }

        public string ChildrenPath
        {
            get { return (string)GetValue(ChildrenPathProperty); }
            set { SetValue(ChildrenPathProperty, value); }
        }

        public bool Expand
        {
            get { return (bool)GetValue(ExpandProperty); }
            set { SetValue(ExpandProperty, value); }
        }

        public bool Check
        {
            get { return (bool)GetValue(CheckProperty); }
            set { SetValue(CheckProperty, value); }
        }

        static MultiSelectTreeView()
        {
            ItemsSourceProperty.OverrideMetadata(typeof(MultiSelectTreeView), new FrameworkPropertyMetadata(null, ItemsSourceChanged, ItemsSourceCoerce));
            ItemTemplateSelectorProperty.OverrideMetadata(typeof(MultiSelectTreeView), new FrameworkPropertyMetadata(new PropertyDataTemplateSelector(), tsChanged));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectTreeView), new FrameworkPropertyMetadata(typeof(MultiSelectTreeView)));
        }

        private static void tsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void KeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MultiSelectTreeView).KeySubject.OnNext((string)e.NewValue);
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var b = ((IEnumerable)e.NewValue).First() is ViewModel.SEObject<object>;
            //if (!b)
            //    (d as MultiSelectTreeView).ItemsSourceSubject.OnNext((IEnumerable)e.NewValue);
        }

        private static object ItemsSourceCoerce(DependencyObject d, object baseValue)
        {
            if (baseValue != null)
            {
                if (((IEnumerable)baseValue).Count() > 0)
                {
                    if (((IEnumerable)baseValue).First() is ChildInteractiveObject<object>)
                        return baseValue;
                    else
                    {
                        (d as MultiSelectTreeView).ItemsSourceSubject.OnNext((IEnumerable)baseValue);
                        return null;
                    }
                }
            }
            return null;
        }

        private static void ChildrenPathChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MultiSelectTreeView).ChildrenPathSubject.OnNext((string)e.NewValue);
        }

        private static void ExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MultiSelectTreeView).ExpandSubject.OnNext((bool)e.NewValue);
        }

        private static void CheckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MultiSelectTreeView).CheckSubject.OnNext((bool)e.NewValue);
        }

        private ISubject<IEnumerable> ItemsSourceSubject = new Subject<IEnumerable>();
        private ISubject<object> SelectedItemSubject = new Subject<object>();
        private ISubject<string> KeySubject = new Subject<string>();
        private ISubject<string> ChildrenPathSubject = new Subject<string>();
        private ISubject<bool> ExpandSubject = new Subject<bool>();
        private ISubject<bool> CheckSubject = new Subject<bool>();

        public MultiSelectTreeView()
        {
            // ItemTemplateSelector =new PropertyDataTemplateSelector();

            var dispatcher = Application.Current.Dispatcher;
            var UI = new System.Reactive.Concurrency.DispatcherScheduler(dispatcher);

            var sets = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Loaded += h, h => Loaded -= h).Select(_ => 0)
                .Take(1)
                .CombineLatest(ChildrenPathSubject.StartWith(Children).DistinctUntilChanged(),
                ItemsSourceSubject.DistinctUntilChanged(),
                (a, children, itemsource) => new { children, itemsource })
                //.CombineLatest(KeySubject.StartWith("Key").DistinctUntilChanged(), (cp, key) => new { cp, key })
                //.CombineLatest(CheckSubject.StartWith(Check).DistinctUntilChanged(), (ci, check) => new { check,ci })
                .Select(init => React(/*init.a.key,*/init.children, init.itemsource, CheckSubject.StartWith(Check).DistinctUntilChanged(), UI, dispatcher))
                .Subscribe(_ =>
                {
                    Dispatcher.InvokeAsync(() => { ItemsSource = _.Items; }, System.Windows.Threading.DispatcherPriority.Background, default);
                });
        }

        public virtual InteractiveCollectionViewModel<object, IConvertible> React(/*string key,*/string childrenpath, IEnumerable enumerable, IObservable<bool> ischecked, System.Reactive.Concurrency.DispatcherScheduler UI, System.Windows.Threading.Dispatcher dispatcher)
        {
            var sx = ObservableChangeSet.Create<object, IConvertible>(cache =>
            {
                foreach (var val in enumerable)
                    cache.AddOrUpdate(val);
                return System.Reactive.Disposables.Disposable.Empty;
            }, GetKey);

            var kx = new InteractiveCollectionViewModel<object, IConvertible>(sx, ChildrenPath, ischecked, ExpandSubject.StartWith(Expand).DistinctUntilChanged());

            kx.GetChecked();
            kx.GetSelectedItem(ischecked).Subscribe(_ =>
            {
                Dispatcher.InvokeAsync(() => SelectedItem = _, System.Windows.Threading.DispatcherPriority.Background, default);
            });
            kx.SelectCheckedChildItems(ischecked, childrenpath).Subscribe(_ =>
             {
                 Dispatcher.InvokeAsync(() => CheckedItems = _, System.Windows.Threading.DispatcherPriority.Background, default);
             });

            AllCheckedItems = kx.Checked;

            //kx.GetSelected().WithLatestFrom(ischecked,(a,b)=>new { a, b }).Subscribe(_=>
            //{
            //    if (@checked.Contains(_) || _.b==false)
            //    {
            //        this.Dispatcher.InvokeAsync(() => SelectedItem = _.a, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //        this.Dispatcher.InvokeAsync(() => CheckedItems = ReflectionHelper.RecursivePropValues(_.a, childrenpath).Cast<object>().Where(a => @checked.Contains(a)).ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //    }
            //});

            //kx.ChildSubject.Where(_=>_.Value.Interaction==Interaction.Select &&((int) _.Value.Value)>0).WithLatestFrom(ischecked, (a, b) => new { a, b }).Subscribe(_ =>
            //{
            //    if (@checked.Contains(_.a.Key) || _.b == false)
            //    {
            //        this.Dispatcher.InvokeAsync(() => SelectedItem = _.a.Key, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //        this.Dispatcher.InvokeAsync(() => CheckedItems = ReflectionHelper.RecursivePropValues(_.a.Key, childrenpath).Cast<object>().Where(a => @checked.Contains(a)).ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //    }
            //});

            //kx.ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check).Subscribe(_ =>
            //{
            //    if (!((bool)_.Value.Value))
            //        if (@checked.Contains(_.Key))
            //        {
            //            @checked.Remove(_.Key);
            //            this.Dispatcher.InvokeAsync(() => SelectedItem = null, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //            this.Dispatcher.InvokeAsync(() => CheckedItems = null, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //        }

            //   else if (((bool)_.Value.Value))
            //            if (@unchecked.Contains(_.Key))
            //            {
            //                @unchecked.Remove(_.Key);
            //                this.Dispatcher.InvokeAsync(() => SelectedItem = _.Key, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //                this.Dispatcher.InvokeAsync(() => CheckedItems = ReflectionHelper.RecursivePropValues(_.Key, childrenpath).Cast<object>().Where(a => @checked.Contains(a)).ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //            }

            //});

            //kx.DoubleClicked.Subscribe(_ =>
            //{
            //    this.Dispatcher.InvokeAsync(() => DoubleClickedItem = _, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //});

            //SelectedItemSubject.Subscribe(_ =>

            //kx.Deleted.Subscribe(_ =>
            //{
            //    this.Dispatcher.InvokeAsync(() => Deleted = _, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            //});
            return kx;
        }

        public virtual IConvertible GetKey(object trade)
        {
            return UtilityHelper.PropertyHelper.GetPropertyValue<IConvertible>(trade, Key);
        }
    }
}