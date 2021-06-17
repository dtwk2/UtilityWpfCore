using DynamicData;
using LiveCharts.Defaults;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for ChartUserControl.xaml
    /// </summary>
    public partial class ChartUserControl : UserControl
    {
        private ReadOnlyObservableCollection<KeyValuePair<string, DateTimePoint>> a;

        public ChartUserControl()
        {
            Random random = new Random();
            InitializeComponent();

            var resource = (Application.Current.FindResource("Characters") as IEnumerable).Cast<Character>().ToArray();

            var x = Observable.Generate(
                Enumerable.Repeat(1, 10000).Select((a, i) => (i, resource[random.Next(0, resource.Length)])).GetEnumerator(),
                a => a.MoveNext(), a => a, a => a.Current.Item2, a => TimeSpan.FromSeconds(0.01 * a.Current.i))
                .Select((a, i) =>
                            new KeyValuePair<Character, DateTimePoint>(a, new DateTimePoint(DateTime.Now, 1d / (i + 1) + random.NextDouble())))
                .ToObservableChangeSet(100)
                .ObserveOnDispatcher()
                .GroupOn(a => a.Key)
                .Transform(aa =>
                {
                    aa.List.Connect().Transform(a => a.Value).Bind(out var dat).Subscribe();

                    return new
                    {
                        Id = aa.GroupKey.First,
                        //Age = aa.GroupKey.Age;
                        Color = aa.GroupKey.Color,
                        Data = dat
                    };
                })
                .Bind(out var a)
                .Subscribe(a =>
                {
                });
            Selector.Id = "Id";
            Selector.Data = "Data";
            Selector.ItemsSource = a;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Selector.Id = Selector.Id == "First" ? "Last" : "First";
        }

        //  private ReadOnlyObservableCollection<PassFail> b;
    }
}