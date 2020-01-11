using LiveCharts.Defaults;
using OxyPlot;
using OxyPlot.Wpf;
using RandomColorGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            //this.SelectChanges<IEnumerable>(nameof(OxyChart.Data)).Subscribe(a =>
            //{

            //});

            ISubject<MultiTimeLineModel> modelSubject = new Subject<MultiTimeLineModel>();
            var modelChanges = this.SelectControlChanges<PlotView>()
                .Take(1)
                .Subscribe(plotView =>
    {
        plotView.Model ??= new PlotModel();
        var model = new MultiTimeLineModel(this.Dispatcher, plotView.Model);
        modelSubject.OnNext(model);

    });



            var data = this.SelectChanges<IEnumerable>(nameof(OxyChart.Data))
                .SubscribeOn(ReactiveUI.RxApp.TaskpoolScheduler)
                .Merge(this.SelectLoads().Select(a => Data).Where(a => a != null))
                .Select(a => a.MakeObservable())
                .Switch()
                .Cast<KeyValuePair<string, DateTimePoint>>();


            modelSubject
                .CombineLatest(data.Select(a => new KeyValuePair<string, (DateTime dt, double d)>(a.Key, (a.Value.DateTime, a.Value.Value))).Buffer(TimeSpan.FromSeconds(0.5)),
                (model, data) => (model, data))
            .Subscribe(a =>
            {
                a.model.OnNext(a.data);
            });

            var itemsSource = this.SelectChanges<IEnumerable>(nameof(OxyChart.ItemsSource));

            //.Cast<string>();



            modelSubject.CombineLatest(itemsSource.Select(cc => (cc as System.Collections.Specialized.INotifyCollectionChanged)?.SelectActions())
                .Merge(this.SelectLoads()?.Select(v => (ItemsSource as System.Collections.Specialized.INotifyCollectionChanged)?.SelectActions())), (model, b) =>
                     (model,
                      items: b))
                .Subscribe(combination =>
                {
                    if (combination.items == default(IObservable<string>))
                        combination.model.Filter(null);
                    else
                        combination.items.Subscribe(a =>
                        {
                           // var itt = ItemsSource.Cast<object>().Select(o => o.GetType().GetProperty(IdProperty).GetValue(o).ToString());
                            HashSet<string> ids = new HashSet<string>();
                            foreach (var item in ItemsSource.Cast<object>())
                            {
                                Color color = (Color)item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(Color))?.GetValue(item);
                                TimeSpan timeSpan = (TimeSpan)item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(TimeSpan))?.GetValue(item);
                                var id = item.GetType().GetProperty(IdProperty).GetValue(item).ToString();
                                ids.Add(id);
                                
                                if (color != default(Color))
                                {
                                    combination.model.OnNext(new KeyValuePair<string, Color>(id, color));
                                }
                                if (timeSpan != default(TimeSpan))
                                {
                                    combination.model.OnNext(new KeyValuePair<string, TimeSpan>(id, timeSpan));
                                }
                            }

                            combination.model.Filter(ids);
                            var colors = combination.model.SelectColors();
                        });
                });

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



}
