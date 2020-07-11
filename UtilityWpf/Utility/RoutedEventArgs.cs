using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilityWpf.Utility
{
    public delegate void RoutedEventHandler<TR>(object sender, RoutedEventArgs<TR> e);

    public class RoutedEventArgs<TEventArgs> : System.Windows.RoutedEventArgs
    {
        public RoutedEventArgs(TEventArgs Target, RoutedEvent routedEvent) :base(routedEvent)
        {
            Value = Target;
        }

        public TEventArgs Value { get; set; }

    }
}
