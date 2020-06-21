using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using static UtilityWpf.View.DateTimeControl;

namespace UtilityWpf.View.Infrastructure
{
    public static  class EventToObservableHelper
    {
        public static IObservable<DateRangeChangeRoutedEventArgs> DateTimeRangeChanges(this DateTimeControl combo) =>
   Observable
.FromEventPattern<DateTimeRangeChangedEventHandler, DateRangeChangeRoutedEventArgs>
(a => combo.DateRangeChange += a, a => combo.DateRangeChange -= a)
.Select(a => a.EventArgs);

    }
}
