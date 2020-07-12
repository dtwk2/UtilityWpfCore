using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf
{
    public static class EventToObservable
    {
        public static IObservable<IList> SelectAddChanges(this Selector selector) =>

            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .Where(a => a.Count > 0);

        public static IObservable<IList> SelectRemoveChanges(this Selector selector) =>
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
           .Select(_ => _.EventArgs);

    }
}
