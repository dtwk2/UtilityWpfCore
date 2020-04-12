using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf
{
    public static class EventToObservable
    {
        public static IObservable<IList> SelectChanges(this Selector combo) =>

      Observable
      .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
      (a => combo.SelectionChanged += a, a => combo.SelectionChanged -= a)
      .Select(a => a.EventArgs.AddedItems);


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
        

   
    }
}
