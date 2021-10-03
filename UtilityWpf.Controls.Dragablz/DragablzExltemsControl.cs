using Dragablz;
using Evan.Wpf;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls.Dragablz
{
    public class DragablzExItemsControl : DragablzItemsControl, ISelector
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyHelper.Register<object>();
        public static readonly DependencyProperty SelectedIndexProperty = DependencyHelper.Register<int>();
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(DragablzVerticalItemsControl));

        public DragablzExItemsControl()
        {
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }


        protected override DependencyObject GetContainerForItemOverride()
        {
            if (base.GetContainerForItemOverride() is not DragablzItem item)
                throw new Exception("s444dfsdf");

            item.MouseDown += Child_MouseDown;
            item.GotFocus += Child_GotFocus;
            item.MouseEnter += DragablzExItemsControl_MouseEnter;
            item.PreviewKeyDown += DragablzExItemsControl_PreviewKeyDown;
            item.PreviewKeyUp += DragablzExItemsControl_PreviewKeyDown;

            foreach (var child in item.FindVisualChildren<FrameworkElement>())
            {
                child.MouseDown += Child_MouseDown;
                child.GotFocus += Child_GotFocus;
            }

            item?
                .WhenAny(a => a.IsSelected, (a) => a)
                .Skip(1)
                .ObserveOnDispatcher()
                .SubscribeOnDispatcher()
                .Subscribe(a =>
                {
                    if (a.Value == false)
                    {
                        return;
                    }
                    var items = Items.OfType<object>().Select(a => ItemContainerGenerator.ContainerFromItem(a)).Cast<DragablzItem>().ToArray();
                    var selected = items.Where(a => a.IsSelected).Select(a => a.Content).ToArray();
                    if (selected.Any() == false)
                        return;

                    int i = 0;
                    int index = -1;
                    foreach (var ditem in items)
                    {
                        if (ditem.Content != item.Content && ditem.IsSelected == true)
                            ditem.IsSelected = false;
                        else if (ditem == item)
                        {
                            index = i;
                        }
                        i++;
                    }

                    selected = items.Where(a => a.IsSelected).ToArray();
                    if (selected.Length != 1)
                    {
                        throw new Exception("Only a single item can ever be selected. Make sure 'Equals' method is correctly implemented.");
                    }
                    SelectedItem = selected.Cast<DragablzItem>().Select(a => a.Content).SingleOrDefault();
                    SelectedIndex = index;
                    RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, selected, new[] { a.Sender.Content }));

                });

            return item;

        }

        private void DragablzExItemsControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        private void DragablzExItemsControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void Child_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DragablzItem dItem)
            {
                dItem.IsSelected = true;
                return;
            }
            if ((e.OriginalSource as Control)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void Child_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DragablzItem dItem)
            {
                dItem.IsSelected = true;
                return;
            }
            if (sender is DragablzItem parent)
                parent.IsSelected = true;
        }

        private void DragablzExItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DragablzItem dragablzItem)
                dragablzItem.IsSelected = true;
        }
    }
}
