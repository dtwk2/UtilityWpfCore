using System;
using System.Collections.Generic;

using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Dragablz;
using Evan.Wpf;
using ReactiveUI;
using UtilityWpf.Abstract;
using UtilityWpf.Attached;

namespace UtilityWpf.Controls
{
    public class DragablzExItemsControl : DragablzItemsControl, ISelector
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyHelper.Register<object?>();
        public static readonly DependencyProperty SelectedIndexProperty = DependencyHelper.Register<int>();
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(DragablzVerticalItemsControl));

        public DragablzExItemsControl()
        {
        }

        public object? SelectedItem
        {
            get { return (object?)GetValue(SelectedItemProperty); }
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
            var item = base.GetContainerForItemOverride();

            (item as DragablzItem).MouseDown += Child_MouseDown;
            (item as DragablzItem).GotFocus += Child_GotFocus;
            (item as DragablzItem).MouseEnter += DragablzExItemsControl_MouseEnter;
            (item as DragablzItem).PreviewKeyDown += DragablzExItemsControl_PreviewKeyDown;
            (item as DragablzItem).PreviewKeyUp += DragablzExItemsControl_PreviewKeyDown;
            foreach (var child in (item as DragablzItem).FindVisualChildren<FrameworkElement>())
            {
                child.MouseDown += Child_MouseDown;
                child.GotFocus += Child_GotFocus;
            }

            (item as DragablzItem)?
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
                    var items = Items.OfType<object>().Select(a => this.ItemContainerGenerator.ContainerFromItem(a)).Cast<DragablzItem>().ToArray();
                    var selected = items.Where(a => a.IsSelected).Select(a => a.Content).ToArray();
                    int i = 0;
                    int index = -1;
                    foreach (var ditem in items)
                    {
                        if (ditem != item && ditem.IsSelected == true)
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
                        throw new Exception("Expected only sinlge item to be selected. Make sure 'Equals' method is correctly implemented.");
                    }
                    SelectedItem = selected.Cast<DragablzItem>().Select(a => a.Content).SingleOrDefault();
                    SelectedIndex = index;
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, selected, new[] { a.Sender.Content }));

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
