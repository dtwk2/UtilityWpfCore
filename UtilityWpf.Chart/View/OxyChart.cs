using LiveCharts.Defaults;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Media;
using UtilityWpf.Abstract;
using UtilityWpf.View;

namespace UtilityWpf.Chart
{



    public class OxyChart : Controlx, IItemsSource
    {
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(OxyChart), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(OxyChart), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable), typeof(OxyChart), new PropertyMetadata(null, Changed));

        static OxyChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChart), new FrameworkPropertyMetadata(typeof(OxyChart)));
        }


        public OxyChart()
        {
            ISubject<MultiTimeLineModel> modelSubject = new Subject<MultiTimeLineModel>();

            var modelChanges = this.SelectControlChanges<PlotView>()
                .Take(1)
                .Subscribe(plotView =>
                {
                    plotView.Model ??= new PlotModel();
                    var model = new MultiTimeLineModel(plotView.Model);
                    modelSubject.OnNext(model);
                });

            var data = this.SelectChanges<IEnumerable>(nameof(OxyChart.Data))
                .SubscribeOn(ReactiveUI.RxApp.TaskpoolScheduler)
                .Merge(this.LoadedChanges().Select(a => Data).Where(a => a != null))
                .Select(a => a.MakeObservable())
                .Switch()
                .Cast<KeyValuePair<string, DateTimePoint>>();

            data.Subscribe(D =>
            {

            });

            modelSubject
                .CombineLatest(data.Select(a => new KeyValuePair<string, (DateTime dt, double d)>(a.Key, (a.Value.DateTime, a.Value.Value))).Buffer(TimeSpan.FromSeconds(0.5)),
                (model, data) => (model, data))
                .Subscribe(a =>
                {
                    a.model.OnNext(a.data);
                });

            var itemsSource = this.SelectChanges<IEnumerable>(nameof(OxyChart.ItemsSource));



            modelSubject
                .CombineLatest(
                itemsSource.Select(cc => cc.MakeObservable()).Merge(this.LoadedChanges()?.Where(a=> ItemsSource != null)
                                                .Select(v => ItemsSource.MakeObservable())), (model, b) =>
                (model,      items: b))
                .ObserveOnDispatcher()
                .Subscribe(combination =>
                {
                    if (combination.items == default(IObservable<string>))
                        combination.model.Filter(null);
                    else
                        combination.items.ObserveOnDispatcher().Subscribe(a =>
                        {
                            // var itt = ItemsSource.Cast<object>().Select(o => o.GetType().GetProperty(IdProperty).GetValue(o).ToString());
                            HashSet<string> ids = new HashSet<string>();
                            foreach (var item in ItemsSource.Cast<object>())
                            {
                                Color color = (Color)item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(Color))?.GetValue(item);
                                TimeSpan timeSpan = (TimeSpan)item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(TimeSpan))?.GetValue(item);
                                var id = item.GetType().GetProperty(Id).GetValue(item).ToString();
                                ids.Add(id);

                                if (color != default)
                                {
                                    combination.model.OnNext(new KeyValuePair<string, Color>(id, color));
                                }
                                if (timeSpan != default)
                                {
                                    combination.model.OnNext(new KeyValuePair<string, TimeSpan>(id, timeSpan));
                                }
                            }

                            combination.model.Filter(ids);
                            var colors = combination.model.SelectColors();
                        },e=>
                        {
                        },
                        ()=>
                        { }
                        );
                });

        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
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
