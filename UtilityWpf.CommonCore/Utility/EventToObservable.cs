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

        ////
        //// Summary:
        ////     Occurs when this System.Windows.FrameworkElement is initialized. This event coincides
        ////     with cases where the value of the System.Windows.FrameworkElement.IsInitialized
        ////     property changes from false (or undefined) to true.
        //[EditorBrowsable(EditorBrowsableState.Advanced)]
        //public event EventHandler Initialized;
        ////
        //// Summary:
        ////     Occurs when the element is laid out, rendered, and ready for interaction.
        //public event RoutedEventHandler Loaded;

    }
}
