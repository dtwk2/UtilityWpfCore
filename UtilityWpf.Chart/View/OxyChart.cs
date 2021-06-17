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
using UtilityHelperEx;
using UtilityWpf.Abstract;
using UtilityWpf.View;

namespace UtilityWpf.Chart
{
    public class OxyChart : Controlx, IItemsSource
    {
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(OxyChart), new PropertyMetadata("Id"));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(OxyChart), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(string), typeof(OxyChart), new PropertyMetadata(null, Changed));

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

            //var data = this.SelectChanges<IEnumerable<KeyValuePair<string, DateTimePoint>>>(nameof(OxyChart.Data))
            //    .SubscribeOn(ReactiveUI.RxApp.TaskpoolScheduler)
            //    .Merge(this.LoadedChanges().Select(a => Data).Where(a => a != null))
            //    .Select(a => a.MakeObservable())
            //    .Switch();

            //data.Subscribe(D =>
            //{
            //});

            var itemsSource = this.SelectChanges<IEnumerable>(nameof(OxyChart.ItemsSource));

            modelSubject
                .CombineLatest(
                itemsSource.Select(cc => cc.MakeObservable()), (model, b) =>
                (model, items: b))
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
                                var aa = item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(TimeSpan));

                                TimeSpan? timeSpan = (TimeSpan?)aa?.GetValue(item);

                                var id = item.GetType().GetProperty(Id).GetValue(item).ToString();
                                ids.Add(id);

                                if (color != default)
                                {
                                    combination.model.OnNext(new KeyValuePair<string, Color>(id, color));
                                }
                                if (timeSpan != default)
                                {
                                    combination.model.OnNext(new KeyValuePair<string, TimeSpan>(id, timeSpan.Value));
                                }

                                var data = item.GetType().GetProperties().FirstOrDefault(a => a.Name == Data)?.GetValue(item) as IEnumerable<DateTimePoint>;

                                data.MakeObservable()
                                    .Select(a => new KeyValuePair<string, (DateTime dt, double d)>(id, (a.DateTime, a.Value))).Buffer(TimeSpan.FromSeconds(0.5))
                                    .Where(a => a.Count > 0)
                                    .Subscribe(data =>
                                     {
                                         combination.model.OnNext(data);
                                     });
                            }

                            combination.model.Filter(ids);
                            var colors = combination.model.SelectColors();
                        }, e =>
                         {
                         },
                        () =>
                        { }
                        );
                });
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
    }
}