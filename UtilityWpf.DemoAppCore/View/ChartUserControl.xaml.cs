using Bogus;
using DynamicData;
using LiveCharts.Defaults;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilityWpf.DemoApp;


namespace UtilityWpf.DemoAppCore.View
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
                Enumerable.Repeat(1, 10000).Select(a => resource[random.Next(0, resource.Length)]).GetEnumerator(),
                a => a.MoveNext(), a => a, a => a.Current, a => TimeSpan.FromSeconds(1))
                .Select((a,i)=>
                            new KeyValuePair<string, DateTimePoint>(a.First, new DateTimePoint(DateTime.Now, 1d / (i) + random.NextDouble())))
                .ToObservableChangeSet(100)
                .ObserveOnDispatcher()
                .Bind(out a)
                .Subscribe(a =>
                {

                });

            Selector.Data = a;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Selector.Id = Selector.Id == "First" ? "Age" : "First";
               
        }

        //  private ReadOnlyObservableCollection<PassFail> b;


    }
}