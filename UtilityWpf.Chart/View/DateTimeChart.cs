﻿using System.Collections;
using System.Windows;
using UtilityWpf.View;

namespace UtilityWpf.Chart
{
    public class DateTimeChart : Controlx
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(DateTimeChart), new PropertyMetadata(null, Changed));

        //private static void SeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.NewValue != null)
        //        (d as DateTimeChart).Dispatcher.InvokeAsync(() =>
        //          ((d as DateTimeChart).CartesianChart).Series = new TimeChartViewModel((dynamic)e.NewValue).SeriesCollection, System.Windows.Threading.DispatcherPriority.Background);
        //}

        public override void OnApplyTemplate()
        {
            //CartesianChart = this.GetTemplateChild("PART_Chart") as LiveCharts.Wpf.CartesianChart;
        }

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public LiveCharts.Wpf.CartesianChart CartesianChart { get; private set; }

        static DateTimeChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeChart), new FrameworkPropertyMetadata(typeof(DateTimeChart)));
            //SeriesProperty.OverrideMetadata(typeof(DateTimeChart), new FrameworkPropertyMetadata(typeof(DateTimeChart), FrameworkPropertyMetadataOptions.None, null, SeriesChanged));
        }

        public DateTimeChart()
        {
            //this.SelectControlChanges<CartesianChart>().CombineLatest(this.SelectChanges<IEnumerable>nameof(Items))
        }
    }
}