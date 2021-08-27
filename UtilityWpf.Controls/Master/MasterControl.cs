using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Evan.Wpf;
using ReactiveUI;
using System.Collections.Specialized;
using System.Reactive.Subjects;
using Dragablz;
using DynamicData;
using Microsoft.Xaml.Behaviors;
using UtilityHelper.NonGeneric;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls
{
    [Flags]
    public enum RemoveOrder
    {
        None,
        Selected = 1,
        Last = 2,
    }


    public class MasterControl : ContentControlx, ISelector, IChange
    {
        [Flags]
        public enum ButtonType
        {
            None = 0, Add = 1, Remove = 2, MoveUp = 4, MoveDown = 8, All = Add | Remove | MoveUp | MoveDown
        }

        public class MovementEventArgs : CollectionEventArgs
        {
            public MovementEventArgs(IReadOnlyCollection<IndexedObject> array, IReadOnlyCollection<IndexedObject> changes, EventType eventType, object? item, int index, RoutedEvent @event) : base(eventType, item, index, @event)
            {
                Objects = array;
                Changes = changes;
            }

            public IReadOnlyCollection<IndexedObject> Objects { get; }
            public IReadOnlyCollection<IndexedObject> Changes { get; }
        }


        public class IndexedObject
        {
            public IndexedObject(object @object, int index, int oldIndex)
            {
                Object = @object;
                Index = index;
                OldIndex = oldIndex;
            }
            public int Index { get; set; }
            public int OldIndex { get; }
            public object Object { get; }
        }


        private Selector Selector => ItemsControl is Selector selector ? selector : throw new Exception($@"The ItemsControl used must be of type {nameof(Selector)} for operation.");

        readonly Subject<Orientation> subject = new();
        readonly Subject<WrapPanel> wrapPanelSubject = new();

        public static readonly DependencyProperty OrientationProperty = DependencyHelper.Register<Orientation>(new PropertyMetadata(Orientation.Horizontal, OrientationChanged));

        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MasterControl).subject.OnNext((Orientation)e.NewValue);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MasterControl), new PropertyMetadata(null));
        private static readonly DependencyProperty ItemsControlProperty = DependencyProperty.Register("ItemsControl", typeof(ItemsControl), typeof(MasterControl), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty = DependencyHelper.Register<IEnumerator>();
        public static readonly DependencyProperty RemoveOrderProperty = DependencyHelper.Register<RemoveOrder>(new PropertyMetadata(RemoveOrder.Selected));
        public static readonly DependencyProperty CountProperty = DependencyHelper.Register<int>();
        public static readonly DependencyProperty ButtonTypesProperty = DependencyHelper.Register<ButtonType>(new PropertyMetadata(ButtonType.All));
        public static readonly RoutedEvent ChangeEvent = EventManager.RegisterRoutedEvent(nameof(Change), RoutingStrategy.Bubble, typeof(CollectionChangedEventHandler), typeof(MasterControl));
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(MasterControl));

        static MasterControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(typeof(MasterControl)));
        }

        public MasterControl()
        {
            // should be on ui thread;
            // SynchronizationContext context = new();
            this.subject
                .CombineLatest(wrapPanelSubject)
                .Where(a => a.Second != null)
                //.SubscribeOn(context)
                //.ObserveOn(context)
                .Subscribe(c =>
                {
                    var (orientation, dockPanel) = c;

                    if (orientation == Orientation.Horizontal)
                    {
                        DockPanel.SetDock(dockPanel, Dock.Right);
                        dockPanel.Orientation = Orientation.Vertical;
                    }
                    else if (orientation == Orientation.Vertical)
                    {
                        DockPanel.SetDock(dockPanel, Dock.Bottom);
                        dockPanel.Orientation = Orientation.Horizontal;
                    }
                });
        }

        #region properties

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public IEnumerator CommandParameter
        {
            get { return (IEnumerator)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public RemoveOrder RemoveOrder
        {
            get { return (RemoveOrder)GetValue(RemoveOrderProperty); }
            set { SetValue(RemoveOrderProperty, value); }
        }

        public ButtonType ButtonTypes
        {
            get { return (ButtonType)GetValue(ButtonTypesProperty); }
            set { SetValue(ButtonTypesProperty, value); }
        }

        public event CollectionChangedEventHandler Change
        {
            add { AddHandler(ChangeEvent, value); }
            remove { RemoveHandler(ChangeEvent, value); }
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        #endregion properties


        public virtual object? SelectedItem
        {
            get
            {
                return ItemsControl switch
                {
                    Selector selector => selector.SelectedItem,
                    ISelector selector => selector.SelectedItem,
                    _ => null,
                };

            }
        }

        public virtual int SelectedIndex
        {
            get
            {
                return ItemsControl switch
                {
                    Selector selector => selector.SelectedIndex,
                    ISelector selector => selector.SelectedIndex,
                    _ => -1,
                };

            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            private set { SetValue(ItemsControlProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            var buttonAdd = this.GetTemplateChild("ButtonPlus") as Button;
            var buttonRemove = this.GetTemplateChild("ButtonMinus") as Button;
            var buttonMoveUp = this.GetTemplateChild("ButtonMoveUp") as Button;
            var buttonMoveDown = this.GetTemplateChild("ButtonMoveDown") as Button;


            buttonAdd.Visibility = ButtonTypes.HasFlag(ButtonType.Add) ? Visibility.Visible : Visibility.Collapsed;
            buttonRemove.Visibility = ButtonTypes.HasFlag(ButtonType.Remove) ? Visibility.Visible : Visibility.Collapsed;
            buttonMoveUp.Visibility = ButtonTypes.HasFlag(ButtonType.MoveUp) ? Visibility.Visible : Visibility.Collapsed;
            buttonMoveDown.Visibility = ButtonTypes.HasFlag(ButtonType.MoveDown) ? Visibility.Visible : Visibility.Collapsed;


            var dockPanel = this.GetTemplateChild("DockPanel1") as DockPanel;
            var wrapPanel = this.GetTemplateChild("WrapPanel1") as WrapPanel;
            wrapPanelSubject.OnNext(wrapPanel);
            buttonAdd.Click += (s, e) => ExecuteAdd();
            buttonRemove.Click += (s, e) => ExecuteRemove();
            buttonMoveUp.Click += (s, e) => ExecuteMoveUp();
            buttonMoveDown.Click += (s, e) => ExecuteMoveDown();

            ItemsControl = (this.Content as ItemsControl) ?? (this.Content as DependencyObject)?.FindVisualChildren<ItemsControl>().SingleOrDefault()!;
            if (ItemsControl != null)
            {
                //this.SetValue(ItemsSourceProperty, itemsControl.ItemsSource);
                ItemsControl.WhenAnyValue(c => c.ItemsSource)

                    .Subscribe(itemsSource =>
                    {
                        if (this.ItemsSource == null)
                            this.ItemsSource = itemsSource;
                    });
            }
            else
                throw new Exception($"Expected content to derive from type of {nameof(ItemsControl)}.");

            if (ItemsControl is ISelector selectionChanged)
            {
                selectionChanged.SelectionChanged += (s, e) =>
                {
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
                    //this.SetValue(MasterControl.CountProperty, selectionChanged.ItemsSource.Count());
                };
            }
            // Unlikely to be both ISelectionChanged and Selector
            if (ItemsControl is Selector selector)
            {
                selector.SelectionChanged += (s, e) =>
                {
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
                    this.SetValue(CountProperty, selector.ItemsSource.Count());
                };
                this.SetValue(CountProperty, selector.ItemsSource.Count());
            }
            else if (ItemsControl.ItemsSource is INotifyCollectionChanged changed)
            {
                changed.CollectionChanged += (s, e) =>
                {
                    this.SetValue(CountProperty, ItemsControl.ItemsSource.Count());
                };
            }
            else
            {
                wrapPanel.DataContext = ItemsControl.ItemsSource;

                ItemsControl
                    .WhenAnyValue(a => a.ItemsSource)
                     .WhereNotNull()
                     .DistinctUntilChanged()
                    .Subscribe(iSource =>
                    {
                        if (iSource is INotifyCollectionChanged changed)
                        {
                            changed.CollectionChanged += (s, e) =>
                            {
                                this.SetValue(CountProperty, iSource.Count());
                            };

                            Count = iSource.Count();
                        }
                        // ItemsSource = iSource;
                    });
            }
            Count = ItemsSource?.Count() ?? 0;

            this.WhenAnyValue(a => a.ItemsSource)

                .Subscribe(a =>
            {
                if (a != null)
                    ItemsControl.ItemsSource = a;
            });

            base.OnApplyTemplate();
        }

        protected virtual void ExecuteAdd()
        {
            //if (CommandParameter == null)
            //    throw new Exception($"{nameof(CommandParameter)} is null");
            if (CommandParameter?.MoveNext() == true)
            {
                if (Content is DragablzItemsControl itemsControl)
                    try
                    {
                        itemsControl.AddToSource(CommandParameter.Current, AddLocationHint.Last);


                    }
                    catch (Exception ex)
                    {

                    }
            }
            else
            {

            }
            RaiseEvent(new CollectionEventArgs(EventType.Add, SelectedItem, SelectedIndex, ChangeEvent));
        }

        protected virtual void ExecuteRemove()
        {
            RaiseEvent(new CollectionChangedEventArgs(ItemsControl.ItemsSource, new[] { SelectedItem }, EventType.Remove, SelectedItem, SelectedIndex, ChangeEvent));

            if (ItemsControl != null)
            {
                if (ItemsControl.ItemsSource is IList collection && collection.Count > 0)
                {
                    if (RemoveOrder == RemoveOrder.Selected && SelectedIndex > -1)
                    {
                        var item = ItemsControl.ItemsSource.Cast<object>().ElementAt(SelectedIndex);
                        collection.RemoveAt(SelectedIndex);
                        RaiseEvent(new CollectionChangedEventArgs(collection, new[] { item }, EventType.Removed, item, SelectedIndex, ChangeEvent));
                    }

                    if (RemoveOrder == RemoveOrder.Last)
                    {
                        var item = ItemsControl.ItemsSource.Cast<object>().Last();
                        collection.RemoveAt(collection.Count - 1);
                        RaiseEvent(new CollectionChangedEventArgs(collection, new[] { item }, EventType.Removed, item, SelectedIndex, ChangeEvent));
                    }
                }
            }
            else
            {
            }

        }

        protected virtual void ExecuteMoveUp()
        {
            var list = GetIndexedObjects();
            List<IndexedObject> changes = new();
            var index = ItemsControl.Items.IndexOf(SelectedItem);
            if (index != 0)
            {
                list[index - 1].Index += 1;
                list[index].Index -= 1;
                changes.Add(list[index]);
            }
            RaiseEvent(new MovementEventArgs(list, changes, EventType.MoveUp, SelectedItem, SelectedIndex, ChangeEvent));
        }

        protected virtual void ExecuteMoveDown()
        {
            var list = GetIndexedObjects();
            List<IndexedObject> changes = new();
            if (SelectedItem != null)
            { }
            var index = ItemsControl.Items.IndexOf(SelectedItem);
            if (index != ItemsControl.Items.Count - 1)
            {
                list[index + 1].Index -= 1;
                list[index].Index += 1;
                changes.Add(list[index]);
            }
            RaiseEvent(new MovementEventArgs(list, changes, EventType.MoveUp, SelectedItem, SelectedIndex, ChangeEvent));
        }

        protected virtual List<IndexedObject> GetIndexedObjects()
        {
            List<IndexedObject> list = new();
            foreach (var item in ItemsControl.Items)
            {
                var oldIndex = ItemsControl.Items.IndexOf(item);
                list.Add(new(item, oldIndex, oldIndex));
            }

            return list;
        }
    }


    public class DisableBehavior : Behavior<MasterControl>
    {
        readonly IDisposable? compositeDisposable;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.RemoveOrder = RemoveOrder.None;
            AssociatedObject.Change += AssociatedObject_Change;
        }

        private void AssociatedObject_Change(object sender, CollectionEventArgs e)
        {
            if (e.EventType == EventType.Remove)
            {
                var element = AssociatedObject.ItemsControl.ItemContainerGenerator.ContainerFromItem(e.Item);
                if (element is UIElement uiElement)
                {
                    uiElement.Opacity = 0.4;
                }
            }

            if (e.EventType == EventType.Add)
            {
                var element = AssociatedObject.ItemsControl.ItemContainerGenerator.ContainerFromItem(e.Item);
                if (element is UIElement uiElement)
                {
                    uiElement.Opacity = 1;
                }
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Change -= AssociatedObject_Change;
            base.OnDetaching();
            compositeDisposable?.Dispose();
        }
    }
}