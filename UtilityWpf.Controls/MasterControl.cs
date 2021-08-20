using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Controls
{
    using Mixins;
    using UtilityWpf.Abstract;

    [Flags]
    public enum RemoveOrder
    {
        None,
        Selected = 1,
        Last = 2,
    }



    public class MasterControl : ContentControlx, ISelectionChanged
    {
        [Flags]
        public enum ButtonType
        {
            None = 0, Add = 1, Remove = 2, MoveUp = 4, MoveDown = 8, All = Add | Remove | MoveUp | MoveDown
        }

        public enum EventType
        {
            Add, Remove, Removed, MoveUp, MoveDown
        }

        public class MovementEventArgs : EventArgs
        {
            public MovementEventArgs(IReadOnlyCollection<IndexedObject> array, IReadOnlyCollection<IndexedObject> changes, EventType eventType, object? item, RoutedEvent @event) : base(eventType, item, @event)
            {
                Objects = array;
                Changes = changes;
            }

            public IReadOnlyCollection<IndexedObject> Objects { get; }
            public IReadOnlyCollection<IndexedObject> Changes { get; }
        }

        public class CollectionChangedEventArgs : EventArgs
        {
            public CollectionChangedEventArgs(IList array, IReadOnlyCollection<object> changes, EventType eventType, object? item, RoutedEvent @event) : base(eventType, item, @event)
            {
                Objects = array;
                Changes = changes;
            }

            public IList Objects { get; }
            public IReadOnlyCollection<object> Changes { get; }
        }


        public class EventArgs : RoutedEventArgs
        {
            public EventArgs(EventType eventType, object? item, RoutedEvent @event) : base(@event)
            {
                EventType = eventType;
                Item = item;
            }

            public EventType EventType { get; }

            public object? Item { get; }
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

        protected ItemsControl itemsControl;
        private Selector Selector => itemsControl is Selector selector ? selector : throw new Exception($@"The ItemsControl used must be of type {nameof(Selector)} for operation.");

        public static readonly DependencyProperty OrientationProperty = DependencyHelper.Register<Orientation>(new PropertyMetadata(Orientation.Horizontal));
        public static readonly DependencyProperty CommandParameterProperty = DependencyHelper.Register<IEnumerator>();
        public static readonly DependencyProperty ItemsSourceProperty = DependencyHelper.Register<IEnumerable>();
        public static readonly DependencyProperty RemoveOrderProperty = DependencyHelper.Register<RemoveOrder>();
        public static readonly DependencyProperty ButtonTypesProperty = DependencyHelper.Register<ButtonType>(new PropertyMetadata(ButtonType.All));
        public static readonly RoutedEvent ChangeEvent = EventManager.RegisterRoutedEvent("Change", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MasterControl));

        public event SelectionChangedEventHandler SelectionChanged;

        static MasterControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(typeof(MasterControl)));
        }

        public MasterControl()
        {
            this.WhenAnyValue(a => a.Orientation)
                .CombineLatest(this.Control<WrapPanel>("WrapPanel1"))
                .Where(a => a.Second != null)
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

        public event RoutedEventHandler Change
        {
            add { AddHandler(ChangeEvent, value); }
            remove { RemoveHandler(ChangeEvent, value); }
        }

        protected virtual object? SelectedItem => itemsControl is Selector selector ? selector.SelectedItem : null;

        protected virtual int? SelectedIndex => itemsControl is Selector selector ? selector.SelectedIndex : null;

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

            buttonAdd.Click += (s, e) =>
            {
                if (CommandParameter == null)
                    throw new Exception($"{nameof(CommandParameter)} is null");
                CommandParameter.MoveNext();
                ExecuteAdd(CommandParameter.Current);
            };

            buttonRemove.Click += (s, e) => ExecuteRemove();
            buttonMoveUp.Click += (s, e) => ExecuteMoveUp();
            buttonMoveDown.Click += (s, e) => ExecuteMoveDown();

            itemsControl = (this.Content as ItemsControl) ?? (this.Content as DependencyObject)?.FindVisualChildren<ItemsControl>().SingleOrDefault()!;
            if (itemsControl != null)
            {
                this.SetValue(ItemsSourceProperty, itemsControl.ItemsSource);
                wrapPanel.DataContext = itemsControl.ItemsSource;
            }
            else
                throw new Exception($"Expected content to derive from type {nameof(ItemsControl)}.");

            if (itemsControl is ISelectionChanged selectionChanged)
            {
                selectionChanged.SelectionChanged += (s, e) => this.SelectionChanged.Invoke(this, e);
            }
            else if (itemsControl is Selector selector)
            {
                selector.SelectionChanged += (s, e) => this.SelectionChanged.Invoke(this, e);
            }

            base.OnApplyTemplate();
        }

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void ExecuteAdd(object parameter)
        {
            RaiseEvent(new EventArgs(EventType.Add, SelectedItem, ChangeEvent));
        }

        protected virtual void ExecuteRemove()
        {
            if (itemsControl != null)
            {
                if (itemsControl.ItemsSource is IList collection && collection.Count > 0)
                {
                    if (RemoveOrder == RemoveOrder.Selected && SelectedIndex > -1)
                    {
                        var item = itemsControl.ItemsSource.Cast<object>().ElementAt(SelectedIndex.Value);
                        collection.RemoveAt(SelectedIndex.Value);
                        RaiseEvent(new CollectionChangedEventArgs(collection, new[] { item }, EventType.Removed, item, ChangeEvent));
                    }

                    if (RemoveOrder == RemoveOrder.Last)
                    {
                        var item = itemsControl.ItemsSource.Cast<object>().Last();
                        collection.RemoveAt(collection.Count - 1);
                        RaiseEvent(new CollectionChangedEventArgs(collection, new[] { item }, EventType.Removed, item, ChangeEvent));
                    }

                }
            }
            else
            {
            }

            RaiseEvent(new EventArgs(EventType.Remove, SelectedItem, ChangeEvent));
        }

        protected virtual void ExecuteMoveUp()
        {
            var list = GetIndexedObjects();
            List<IndexedObject> changes = new();
            var index = itemsControl.Items.IndexOf(SelectedItem);
            if (index != 0)
            {

                list[index - 1].Index += 1;
                list[index].Index -= 1;
                changes.Add(list[index]);
            }
            RaiseEvent(new MovementEventArgs(list, changes, EventType.MoveUp, SelectedItem, ChangeEvent));
        }

        protected virtual void ExecuteMoveDown()
        {
            var list = GetIndexedObjects();
            List<IndexedObject> changes = new();
            if (SelectedItem != null)
            { }
            var index = itemsControl.Items.IndexOf(SelectedItem);
            if (index != itemsControl.Items.Count - 1)
            {

                list[index + 1].Index -= 1;
                list[index].Index += 1;
                changes.Add(list[index]);
            }
            RaiseEvent(new MovementEventArgs(list, changes, EventType.MoveUp, SelectedItem, ChangeEvent));
        }

        protected virtual List<IndexedObject> GetIndexedObjects()
        {
            List<IndexedObject> list = new();
            //List<IndexedObject> changes = new();
            foreach (var item in itemsControl.Items)
            {
                var oldIndex = itemsControl.Items.IndexOf(item);
                list.Add(new(item, oldIndex, oldIndex));
            }

            return list;
        }
    }
}