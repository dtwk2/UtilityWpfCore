using LiveCharts.Defaults;
using OxyPlot;
using OxyPlot.Wpf;
using RandomColorGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

namespace UtilityWpf.Chart
{
    public class MultiTimeLineModel : IObserver<IEnumerable<KeyValuePair<string, (DateTime date, double value)>>>, IObserver<KeyValuePair<string, (DateTime date, double value)>>, IObserver<KeyValuePair<string, Color>>
    {
        Dictionary<string, CheckableList> DataPoints;
        private Dispatcher dispatcher;
        private PlotModel plotModel;
        object lck = new object();
        private readonly Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>();
        ISubject<Unit> refreshes = new Subject<Unit>();


        public MultiTimeLineModel(Dispatcher dispatcher, PlotModel model)
        {
            this.dispatcher = dispatcher;
            this.plotModel = model;
            model.Axes.Add(new OxyPlot.Axes.DateTimeAxis { });
            DataPoints = GetDataPoints();

            refreshes.Buffer(TimeSpan.FromSeconds(1)).Subscribe(list =>
            {
                lock (lck)
                {
                    var series = SelectSeries().AsParallel().ToArray();

                    this.dispatcher.BeginInvoke(() =>
                    {
                        plotModel.Series.Clear();
                        foreach (var serie in series)
                        {
                            plotModel.Series.Add(serie);
                        }
                        plotModel.InvalidatePlot(true);
                    });
                }
            });
        }


        private IEnumerable<OxyPlot.Series.LineSeries> SelectSeries()
        {
            foreach (var dataPoint in DataPoints.Where(a => a.Value.Check))
            {
                var points = dataPoint.Value.DataPoints
                     .OrderBy(dt => dt.DateTime);

                var build = Build(points, dataPoint.Key.ToString(), SetColor(dataPoint.Key.ToString()));

                yield return build;

            }

            //var build = colorDictionary.ContainsKey(dataPoint.Key.ToString());

            if (ShowAll)
            {
                var allPoints = DataPoints.Where(a => a.Value.Check).SelectMany(a => a.Value.DataPoints)
                           .OrderBy(dt => dt.DateTime)
                           .GroupBy(a => a.DateTime)
                         .Select(xy0 => new DateTimePoint(xy0.Key, xy0.Sum(l => l.Value)));
                yield return Build(allPoints, "All", Colors.Black);
            }

        }

        private Color SetColor(string v)
        {
            if (colorDictionary.ContainsKey(v) == false)
            {
                var color = RandomColor.GetColor(ColorScheme.Random, Luminosity.Bright);
                int max = 100, i = 0;
                while (colorDictionary.Values.Contains(color))
                {
                    color = RandomColor.GetColor(ColorScheme.Random, Luminosity.Bright);
                    if (++i > max)
                    {
                        color = Colors.Gray;
                        break;
                    }
                }
                colorDictionary[v] = color;

            }

            return colorDictionary[v];
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
                    Add(item);
                }

                Refresh();
            }
        }

        public void OnNext(KeyValuePair<string, (DateTime date, double value)> item)
        {
            lock (lck)
            {
                Add(item);
                Refresh();
            }
        }


        private void Add(KeyValuePair<string, (DateTime date, double value)> item)
        {
            if (!DataPoints.ContainsKey(item.Key))
                DataPoints[item.Key] = new CheckableList();
            var newdp = new DateTimePoint(item.Value.date, item.Value.value);

            DataPoints[item.Key].DataPoints.Add(newdp);
        }

        private void Refresh()
        {
            refreshes.OnNext(new Unit());

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

        }

        public IEnumerable<KeyValuePair<string, Color>> SelectColors()
        {

            foreach (var series in plotModel.Series)
            {
                yield return new KeyValuePair<string, Color>(series.Title, (series as OxyPlot.Series.LineSeries).Color.ToColor());
            }

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

        public static OxyPlot.Series.LineSeries Build(IEnumerable<DateTimePoint> coll, string key, Color color)
        {
            var lser = new OxyPlot.Series.LineSeries
            {
                Title = key,
                StrokeThickness = 1,
                Color = color.ToOxyColor(),
                MarkerSize = 3,
                ItemsSource = coll,
                MarkerType = OxyPlot.MarkerType.Plus,
                DataFieldX = nameof(DateTimePoint.DateTime),
                DataFieldY = nameof(DateTimePoint.Value)
            };

            return lser;
        }

        public void OnNext(KeyValuePair<string, Color> kvp)
        {
            if (colorDictionary.ContainsKey(kvp.Key) == false || colorDictionary[kvp.Key] != kvp.Value)
            {
                colorDictionary[kvp.Key] = kvp.Value;
                Refresh();
            }
        }
    }
}
