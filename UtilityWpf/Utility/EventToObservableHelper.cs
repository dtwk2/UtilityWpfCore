using deniszykov.TypeConversion;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using UtilityWpf.Abstract;

namespace UtilityWpf
{
    public static class EventToObservableHelper
    {
        public static IObservable<EventArgs> SelectCompletions(this Storyboard storyboard) =>

            Observable
            .FromEventPattern<EventHandler, EventArgs>
            (a => storyboard.Completed += a, a => storyboard.Completed -= a)
            .Select(a => a.EventArgs);


        public static IObservable<IList> SelectSelectionAddChanges(this Selector selector) =>

            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .Where(a => a.Count > 0);

        public static IObservable<object> SelectSingleSelectionChanges(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .Where(a => a.Count == 1)
            .Select(a => a.Cast<object>().Single());

        public static IObservable<object> SelectSingleSelectionChanges(this ISelector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .Where(a => a.Count == 1)
            .Select(a => a.Cast<object>().Single());

        public static IObservable<IList> SelectSelectionRemoveChanges(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.RemovedItems)
            .Where(a => a.Count > 0);

        public static IObservable<RoutedEventArgs> LoadedChanges(this FrameworkElement combo) =>
            Observable
            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>
            (a => combo.Loaded += a, a => combo.Loaded -= a)
            .Select(a => a.EventArgs);

        public static IObservable<RoutedEventArgs> VisibleChanges(this FrameworkElement combo) =>
            Observable
            .FromEventPattern<DependencyPropertyChangedEventHandler, RoutedEventArgs>
            (a => combo.IsVisibleChanged += a, a => combo.IsVisibleChanged -= a)
            .Select(a => a.EventArgs);

        public static IObservable<ScrollChangedEventArgs> ScrollChanges(this ScrollViewer combo) =>
         Observable
            .FromEventPattern<ScrollChangedEventHandler, ScrollChangedEventArgs>
            (a => combo.ScrollChanged += a, a => combo.ScrollChanged -= a)
            .Select(a => a.EventArgs);

        public static IObservable<ExitEventArgs> GetExitEvents(this Application app) =>
            Observable
            .FromEventPattern<ExitEventHandler, ExitEventArgs>(h => app.Exit += h, h => app.Exit -= h)
           .Select(a => a.EventArgs);


        public static IObservable<object> ToChanges(this Selector selector) =>
            from change in (from a in Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                            select a.EventArgs.AddedItems.Cast<object>().SingleOrDefault())
            .StartWith(selector.SelectedItem)
            where change != null
            select change;

        public static IObservable<RoutedEventArgs> ToClicks(this Button selector) => from x in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => selector.Click += a, a => selector.Click -= a)
                                                                                     select x.EventArgs;


        public static IObservable<bool?> ToChanges(this ToggleButton toggleButton)
        {
            return (from a in (from a in SelectChecked()
                               select a).Merge(from a in SelectUnchecked()
                                               select a)
                    select a.IsChecked)
                    .StartWith(toggleButton.IsChecked)
                    .DistinctUntilChanged();

            IObservable<ToggleButton> SelectChecked() => from es in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => toggleButton.Checked += a, a => toggleButton.Checked -= a)
                                                         select es.Sender as ToggleButton;

            IObservable<ToggleButton> SelectUnchecked() => from es in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => toggleButton.Unchecked += a, a => toggleButton.Unchecked -= a)
                                                           select es.Sender as ToggleButton;
        }


        public static IObservable<(double h, double v)> ToDeltas(this Thumb thumb) => from es in Observable.FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(a => thumb.DragDelta += a, a => thumb.DragDelta -= a)
                                                                                      select (es.EventArgs.HorizontalChange, es.EventArgs.VerticalChange);




        public static IObservable<T> SelectItemChanges<T>(this ComboBox comboBox)
        {
            var selectionChanged = comboBox.Events().SelectionChanged;
            var conversionProvider = new TypeConversionProvider();
            // If using ComboBoxItems 
            var comboBoxItems = selectionChanged
          .SelectMany(a => a.AddedItems.OfType<ContentControl>())
          .StartWith(comboBox.SelectedItem as ContentControl)
          .Where(a => a != null)
          .Select(a => NewMethod2(a.Content))
            .Where(a => a.Equals(default(T)) == false);

            // If using type directly
            var directItems = selectionChanged
          .SelectMany(a => a.AddedItems.OfType<T>())
          .StartWith(NewMethod(comboBox.SelectedItem))
          .Where(a => a.Equals(default(T)) == false);

            // If using type indirectly
            var indirectItems = selectionChanged
          .SelectMany(a => a.AddedItems.Cast<object>().Select(a => conversionProvider.TryConvert<object, T>(a, out T t2) ? t2 : default))
          .StartWith(NewMethod2(comboBox.SelectedItem))
          .Where(a => a.Equals(default(T)) == false);

            var c = comboBoxItems.Amb(directItems).Amb(indirectItems);

            return c;

            static T NewMethod(object selectedItem)
            {
                return selectedItem is T t ? t : default;
            }

            T NewMethod2(object selectedItem)
            {
                return conversionProvider.TryConvert(selectedItem, out T t2) ? t2 : default;
            }
        }


        public static IObservable<bool> SelectToggleChanges(this ToggleButton toggleButton, bool defaultValue = false)
        {
            return toggleButton.Events()
                .Checked.Select(a => true).Merge(toggleButton.Events()
                .Unchecked.Select(a => false))
                .StartWith(toggleButton.IsChecked ?? defaultValue);
        }

        public static IObservable<string> ToThrottledObservable(this TextBox textBox)
        {
            return Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                   s => textBox.TextChanged += s,
                   s => textBox.TextChanged -= s)
                .Select(evt => textBox.Text) // better to select on the UI thread
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
        }



        public static IObservable<object> ToSelectedValueChanges(this Selector selector) =>

            Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
                    (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                .Select(a => selector.SelectedValue).StartWith(selector.SelectedValue)
                .WhereNotNull();

        public static IObservable<T> ToSelectedValueChanges<T>(this Selector selector) => selector.ToSelectedValueChanges().Cast<T>();





        //public static IObservable<ClickRoutedEventArgs<object>> SelectClicks(this CollectionView buttonsItemsControl)
        //{
        //    return Observable.FromEventPattern<ClickRoutedEventHandler<object>, ClickRoutedEventArgs<object>>(
        //           a => buttonsItemsControl.ButtonClick += a,
        //           a => buttonsItemsControl.ButtonClick -= a)
        //        .Select(a => a.EventArgs);
        //}

        public static IObservable<bool> ToThrottledObservable(this ToggleButton toggleButton)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    s => toggleButton.Checked += s,
                    s => toggleButton.Checked -= s)
                .Select(evt => true)
                .Merge(Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                        s => toggleButton.Unchecked += s,
                        s => toggleButton.Unchecked -= s)
                    .Select(evt => false))
                // better to select on the UI thread
                .Throttle(TimeSpan.FromMilliseconds(500))
                .StartWith(toggleButton.IsChecked ?? false)
                .DistinctUntilChanged();
        }

        public static Task<EventPattern<object>> ToTask(this DoubleAnimation animation)
        {
            return Observable.FromEventPattern(a => animation.Completed += a, a => animation.Completed -= a).Take(1).ToTask();
        }
    }
}