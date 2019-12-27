using LiveCharts.Defaults;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UtilityWpf.Abstract;
using UtilityWpf.View;

namespace UtilityWpf.Chart
{

    public class OxyChartSelector : MasterDetailCheckView
    {
        static OxyChartSelector()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChartSelector), new FrameworkPropertyMetadata(typeof(OxyChartSelector)));
        }

        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable), typeof(OxyChartSelector), new PropertyMetadata(null, Changed));

        public OxyChartSelector()
        {
            var oxyChart = new OxyChart();

            DetailView = oxyChart;

            this.Loaded += OxyChartSelector_Loaded;

        }

        private void OxyChartSelector_Loaded(object sender, RoutedEventArgs e)
        {
            (DetailView as OxyChart).Data = Data;
            (DetailView as OxyChart).IdProperty = Id;
        }
    }



    public class OxyChart : Controlx, IItemsSource
    {
        static OxyChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChart), new FrameworkPropertyMetadata(typeof(OxyChart)));
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(OxyChart), new PropertyMetadata(null, Changed));


        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable), typeof(OxyChart), new PropertyMetadata(null, Changed));




        public string IdProperty
        {
            get { return (string)GetValue(IdPropertyProperty); }
            set { SetValue(IdPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IdProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdPropertyProperty =
            DependencyProperty.Register("IdProperty", typeof(string), typeof(OxyChart), new PropertyMetadata(null));



        public OxyChart()
        {
            this.SelectChanges<IEnumerable>(nameof(OxyChart.Data)).Subscribe(a =>
            {

            });

            var data = this.SelectChanges<IEnumerable>(nameof(OxyChart.Data))
                .Merge(this.SelectLoads().Select(a => Data).Where(a => a != null))
                .Select(a => a.MakeObservable())
                .Switch()
                .Cast<KeyValuePair<string, DateTimePoint>>();


            this.SelectControlChanges<PlotView>()
                .Select(plotView =>
            {
                plotView.Model = plotView.Model ?? new PlotModel();
                var model = new MultiTimeLineModel(this.Dispatcher, plotView.Model);
                return model;

            }).CombineLatest(data.Select(a => new KeyValuePair<string, (DateTime dt, double d)>(a.Key, (a.Value.DateTime, a.Value.Value))).Buffer(TimeSpan.FromSeconds(0.5)), (a, b) => (a, b))
            .Subscribe(a =>
            {
                a.a.OnNext(a.b);
            });

            var itemsSource = this.SelectChanges<IEnumerable>(nameof(OxyChart.ItemsSource));

            //.Cast<string>();

            this.SelectControlChanges<PlotView>()
                .Select(plotView =>
                {
                    plotView.Model = plotView.Model ?? new PlotModel();
                    var model = new MultiTimeLineModel(this.Dispatcher, plotView.Model);
                    return model;

                }).CombineLatest(itemsSource.Select(cc=>cc?.MakeObservable()).Merge(this.SelectLoads()?.Select(v => ItemsSource.MakeObservable())), (model, b) =>
                (model,
                 items: b))
                .Subscribe(combination =>
                {
                    if (combination.items == default(IObservable<string>))
                        combination.model.Filter(null);
                    else
                        combination.items.Subscribe(a =>
                        {
                            var itt = ItemsSource.Cast<object>().Select(o => o.GetType().GetProperty(IdProperty).GetValue(o).ToString());
                            combination.model.Filter(new HashSet<string>(itt));
                        });
                });



            //this.SelectControlChanges<MasterDetailCheckView>().Subscribe(checkView =>
            //{
            //    checkView.Col

            //});



        }


    }

    class CheckableList
    {
        public CheckableList(bool check, List<DateTimePoint> dataPoints)
        {
            this.Check = check;
            this.DataPoints = dataPoints;
        }

        public CheckableList(bool check = false) : this(check, new List<DateTimePoint>())
        {
        }

        public List<DateTimePoint> DataPoints { get; set; } = new List<DateTimePoint>();
        public bool Check { get; set; }
    }


    public class MultiTimeLineModel : IObserver<IEnumerable<KeyValuePair<string, (DateTime date, double value)>>>, IObserver<KeyValuePair<string, (DateTime date, double value)>>
    {
        Dictionary<string, CheckableList> DataPoints;
        private Dispatcher dispatcher;
        private PlotModel plotModel;
        object lck = new object();


        public MultiTimeLineModel(Dispatcher dispatcher, PlotModel model)
        {
            this.dispatcher = dispatcher;
            this.plotModel = model;
            model.Axes.Add(new OxyPlot.Axes.DateTimeAxis { });
            DataPoints = GetDataPoints();
        }




        Dictionary<string, CheckableList> GetDataPoints()
        {
            return new Dictionary<string, CheckableList>();
        }

        public bool ShowAll { get; set; } = false;

        public void OnNext(IEnumerable<KeyValuePair<string, (DateTime date, double value)>> enumerable)
        {
            lock (lck)
            {
                foreach (var item in enumerable)
                {
                    Sdf(item);
                }

                Refresh();
            }
        }

        public void OnNext(KeyValuePair<string, (DateTime date, double value)> item)
        {
            lock (lck)
            {
                Sdf(item);
                Refresh();
            }
        }


        private void Sdf(KeyValuePair<string, (DateTime date, double value)> item)
        {
            if (!DataPoints.ContainsKey(item.Key))
                DataPoints[item.Key] = new CheckableList();
            var newdp = new DateTimePoint(item.Value.date, item.Value.value);

            DataPoints[item.Key].DataPoints.Add(newdp);
        }

        private void Refresh()
        {

            this.dispatcher.BeginInvoke(() =>
            {
                lock (lck)
                {
                    plotModel.Series.Clear();
                    foreach (var dataPoint in DataPoints.Where(a => a.Value.Check))
                    {
                        var points = dataPoint.Value.DataPoints
                             .OrderBy(dt => dt.DateTime);

                        plotModel.Series.Add(Build(points, dataPoint.Key.ToString()));
                    }

                    if (ShowAll)
                    {
                        var allPoints = DataPoints.Where(a => a.Value.Check).SelectMany(a => a.Value.DataPoints)
                                   .OrderBy(dt => dt.DateTime)
                                   .GroupBy(a => a.DateTime)
                                 .Select(xy0 => new DateTimePoint(xy0.Key, xy0.Sum(l => l.Value)));
                        plotModel.Series.Add(Build(allPoints, "All"));
                    }
                    plotModel.InvalidatePlot(true);
                }
            });
        }

        public void Filter(ISet<string> names)
        {
            Predicate<string> predicate = names == null ? new Predicate<string>(s => true) : s => names.Contains(s);

            foreach (var kvp in DataPoints)
            {
                kvp.Value.Check = predicate(kvp.Key);
            }
            lock (lck)
            {
                Refresh();
            }
            //dispatcher.Invoke(() =>
            //{
            //foreach (var title in plotModel.Series.Select(s => s.Title).Except(names))
            //{
            //    var series = plotModel.Series.First(s => s.Title.Equals(title));
            //    //DataPoints.Add(title, series);
            //    plotModel.Series.Remove(series);
            //}

            //foreach (var title in names.Except(plotModel.Series.Select(s => s.Title)))
            //{
            //    if (DataPoints.ContainsKey(title))
            //    {
            //        plotModel.Series.Add(DataPoints[title].DataPoints);
            //        dictionary.Remove(title);
            //    }
            //}


            //DataPoints = GetDataPoints();
            //plotModel.InvalidatePlot(true);
            //});
        }

        public void Reset()
        {
            dispatcher.Invoke(() =>
            {
                while (plotModel.Series.Any())
                    plotModel.Series.Remove(plotModel.Series.First());
                DataPoints = GetDataPoints();
                plotModel.InvalidatePlot(true);
            });
        }

        public void Remove(ISet<string> names)
        {
            dispatcher.Invoke(() =>
            {
                while (plotModel.Series.Any(s => names.Contains(s.Title)))
                {
                    plotModel.Series.Remove(plotModel.Series.First());
                }
                DataPoints = GetDataPoints();
                plotModel.InvalidatePlot(true);
            });
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }


        public static OxyPlot.Series.LineSeries Build(IEnumerable<DateTimePoint> coll, string key)
        {
            var lser = new OxyPlot.Series.LineSeries
            {
                Title = key,
                StrokeThickness = 1,
                //Color = GetColor(key),
                MarkerSize = 3,
                ItemsSource = coll,
                MarkerType = OxyPlot.MarkerType.Plus,
                DataFieldX = nameof(DateTimePoint.DateTime),
                DataFieldY = nameof(DateTimePoint.Value)
            };

            return lser;
        }





    }
}
