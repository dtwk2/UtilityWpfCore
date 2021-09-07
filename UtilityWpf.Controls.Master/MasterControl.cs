using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Evan.Wpf;
using ReactiveUI;
using DynamicData;
using Microsoft.Xaml.Behaviors;
using UtilityWpf.Abstract;
using UtilityWpf.Mixins;
using System.Windows.Media.Animation;

namespace UtilityWpf.Controls.Master
{
    [Flags]
    public enum RemoveOrder
    {
        None,
        Selected = 1,
        Last = 2,
    }


    public class MasterControl : ItemsContentControl, IChange
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

        public static readonly DependencyProperty CommandParameterProperty = DependencyHelper.Register<IEnumerator>();
        public static readonly DependencyProperty RemoveOrderProperty = DependencyHelper.Register<RemoveOrder>(new PropertyMetadata(RemoveOrder.Selected));
        public static readonly DependencyProperty ButtonTypesProperty = DependencyHelper.Register<ButtonType>(new PropertyMetadata(ButtonType.All));
        public static readonly RoutedEvent ChangeEvent = EventManager.RegisterRoutedEvent(nameof(Change), RoutingStrategy.Bubble, typeof(CollectionChangedEventHandler), typeof(MasterControl));

        static MasterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(typeof(MasterControl)));
        }

        public MasterControl()
        {
        }

        #region properties

        public IEnumerator CommandParameter
        {
            get { return (IEnumerator)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
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

        #endregion properties

        public override void OnApplyTemplate()
        {
            var buttonAdd = this.GetTemplateChild("ButtonPlus") as Button;
            var buttonRemove = this.GetTemplateChild("ButtonMinus") as Button;
            var buttonMoveUp = this.GetTemplateChild("ButtonMoveUp") as Button;
            var buttonMoveDown = this.GetTemplateChild("ButtonMoveDown") as Button;


            this.Observable<ButtonType>().Subscribe(buttonType =>
           {
               buttonAdd.Visibility = buttonType.HasFlag(ButtonType.Add) ? Visibility.Visible : Visibility.Collapsed;
               buttonRemove.Visibility = buttonType.HasFlag(ButtonType.Remove) ? Visibility.Visible : Visibility.Collapsed;
               buttonMoveUp.Visibility = buttonType.HasFlag(ButtonType.MoveUp) ? Visibility.Visible : Visibility.Collapsed;
               buttonMoveDown.Visibility = buttonType.HasFlag(ButtonType.MoveDown) ? Visibility.Visible : Visibility.Collapsed;
           });

            buttonAdd.Click += (s, e) => ExecuteAdd();
            buttonRemove.Click += (s, e) => ExecuteRemove();
            buttonMoveUp.Click += (s, e) => ExecuteMoveUp();
            buttonMoveDown.Click += (s, e) => ExecuteMoveDown();
            base.OnApplyTemplate();
        }

        protected virtual void ExecuteAdd()
        {
            //if (CommandParameter?.MoveNext() == true)
            //{
            //    if (Content is DragablzItemsControl itemsControl)
            //        try
            //        {
            //            itemsControl.AddToSource(CommandParameter.Current, AddLocationHint.Last);


            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //}
            //else
            //{

            //}
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
                    Storyboard myStoryboard = OpacityAnimation(uiElement, 0.4);
                    myStoryboard.Begin();
                }
            }

            if (e.EventType == EventType.Add)
            {
                var element = AssociatedObject.ItemsControl.ItemContainerGenerator.ContainerFromItem(e.Item);
                if (element is UIElement uiElement)
                {
                    Storyboard myStoryboard = OpacityAnimation(uiElement, 1);
                    myStoryboard.Begin();
                }
            }

            static Storyboard OpacityAnimation(UIElement uiElement, double opacity)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    To = opacity,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300))
                };
                Storyboard.SetTarget(opacityAnimation, uiElement);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard myStoryboard = new Storyboard();
                myStoryboard.Children.Add(opacityAnimation);
                return myStoryboard;
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