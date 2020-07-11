using Evan.Wpf;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityEnum;
using UtilityWpf.Utility;
using UtilityWpf.View.Extrinsic;
using UtilityWpf.View.Infrastructure;

namespace UtilityWpf.View
{
    public class TimeSpanControl : Controlx
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<(decimal value, TimeInterval timeInterval)>), typeof(TimeSpanControl));

        public static DependencyProperty TimeIntervalProperty = DependencyHelper.Register<TimeInterval>();

        public static DependencyProperty ValueProperty = DependencyHelper.Register<decimal>();

        static TimeSpanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanControl), new FrameworkPropertyMetadata(typeof(TimeSpanControl)));
        }

        public TimeSpanControl()
        {
            this.SelectControlChanges<ComboBox>().Subscribe(a =>
            {
                a.ItemsSource = Enum.GetValues(typeof(TimeInterval));

            });
        }

        public event RoutedEventHandler<(decimal value, TimeInterval timeInterval)> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            this.SelectControlChanges<ComboBox>().SelectMany(a => a.SelectAddChanges().Select(a => a.Cast<TimeInterval>().First())).StartWith((TimeInterval)this.GetValue(TimeIntervalProperty))
                      .CombineLatest(this.SelectControlChanges<SpinnerControl>().SelectMany(a => a.ValueChanges()).StartWith((decimal)this.GetValue(ValueProperty)), (a, b) => (b, a))
                      .DistinctUntilChanged()
                .Subscribe(a =>
                {
                    this.SetValue(TimeIntervalProperty, a.a);
                    this.SetValue(ValueProperty, a.b);
                    RaiseEvent(new RoutedEventArgs<(decimal value, TimeInterval timeInterval)>(a, ValueChangedEvent));
                });

            this.SelectControlChanges<SpinnerControl>().CombineLatest(this.SelectChanges("Value").Cast<decimal>().StartWith(1M).DistinctUntilChanged(), (a, b) => (a, b))
    .Subscribe(a =>
    {
        a.a.Value = a.b;
    });

            this.SelectControlChanges<ComboBox>().CombineLatest(this.SelectChanges("TimeInterval").Cast<TimeInterval>().StartWith(TimeInterval.Second).DistinctUntilChanged(), (a, b) => (a, b))
                .Subscribe(a =>
                {
                    a.a.SelectedItem = a.b;
                });

            base.OnApplyTemplate();


        }
    }
}
