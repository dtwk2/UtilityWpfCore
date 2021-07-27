// Copyright 2012 lapthorn.net.
//
// This software is provided "as is" without a warranty of any kind. All
// express or implied conditions, representations and warranties, including
// any implied warranty of merchantability, fitness for a particular purpose
// or non-infringement, are hereby excluded. lapthorn.net and its licensors
// shall not be liable for any damages suffered by licensee as a result of
// using the software. In no event will lapthorn.net be liable for any
// lost revenue, profit or data, or for direct, indirect, special,
// consequential, incidental or punitive damages, however caused and regardless
// of the theory of liability, arising out of the use of or inability to use
// software, even if lapthorn.net has been advised of the possibility of
// such damages.

using System;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls.Extrinsic
{
    public class ShortTimeSpanControl : Control
    {
        protected static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(int), typeof(ShortTimeSpanControl), new UIPropertyMetadata(0, OnSecondsChanged));

        protected static readonly DependencyProperty MinutesProperty = DependencyProperty.Register("Minutes", typeof(int), typeof(ShortTimeSpanControl), new UIPropertyMetadata(0, OnMinutesChanged));

        protected static readonly DependencyProperty HoursProperty = DependencyProperty.Register("Hours", typeof(int), typeof(ShortTimeSpanControl), new UIPropertyMetadata(0, OnHoursChanged));

        protected static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(TimeSpan), typeof(ShortTimeSpanControl), new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<TimeSpan>), typeof(ShortTimeSpanControl));

        private static readonly RoutedEvent HoursChangedEvent = EventManager.RegisterRoutedEvent("HoursChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ShortTimeSpanControl));

        private static readonly RoutedEvent MinutesChangedEvent = EventManager.RegisterRoutedEvent("MinutesChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ShortTimeSpanControl));

        private static readonly RoutedEvent SecondsChangedEvent = EventManager.RegisterRoutedEvent("SecondsChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ShortTimeSpanControl));

        static ShortTimeSpanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShortTimeSpanControl), new FrameworkPropertyMetadata(typeof(ShortTimeSpanControl)));
        }

        public ShortTimeSpanControl()
        {
        }

        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnMinutesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as ShortTimeSpanControl;
            if (d == null)
                return;

            var oldValue = (int)args.OldValue;
            var newValue = (int)args.NewValue;

            //  make sure we don't get into a loop.
            if (oldValue != newValue)
            {
                control.Value = new TimeSpan(control.Value.Hours, newValue, control.Value.Seconds);
            }

            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue, MinutesChangedEvent);
            control.RaiseEvent(e);
        }

        public event RoutedPropertyChangedEventHandler<TimeSpan> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> HoursChanged
        {
            add { AddHandler(HoursChangedEvent, value); }
            remove { RemoveHandler(HoursChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> MinutesChanged
        {
            add { AddHandler(MinutesChangedEvent, value); }
            remove { RemoveHandler(MinutesChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> SecondsChanged
        {
            add { AddHandler(SecondsChangedEvent, value); }
            remove { RemoveHandler(SecondsChangedEvent, value); }
        }

        private static void OnHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as ShortTimeSpanControl;
            if (d == null)
                return;

            var oldValue = (int)args.OldValue;
            var newValue = (int)args.NewValue;

            //  make sure we don't get into a loop.
            if (oldValue != newValue)
            {
                control.Value = new TimeSpan(newValue, control.Value.Minutes, control.Value.Seconds);
            }
            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue, HoursChangedEvent);
            control.RaiseEvent(e);
        }

        private static void OnSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as ShortTimeSpanControl;
            if (d == null)
                return;

            var oldValue = (int)args.OldValue;
            var newValue = (int)args.NewValue;

            //  make sure we don't get into a loop.
            if (oldValue != newValue)
            {
                control.Value = new TimeSpan(control.Value.Hours, control.Value.Minutes, newValue);
            }

            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue, SecondsChangedEvent);
            control.RaiseEvent(e);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as ShortTimeSpanControl;
            if (d == null)
                return;

            var oldValue = (TimeSpan)args.OldValue;
            var newValue = (TimeSpan)args.NewValue;

            //  ensure we don't get into a loop with the 4 properties changing
            //  by only changing the value if it has changed.

            if (oldValue != newValue)
            {
                control.Hours = newValue.Hours;
                control.Minutes = newValue.Minutes;
                control.Seconds = newValue.Seconds;
            }

            var e = new RoutedPropertyChangedEventArgs<TimeSpan>(oldValue, newValue, ValueChangedEvent);

            control.RaiseEvent(e);
        }
    }
}