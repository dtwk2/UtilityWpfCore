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
using UtilityWpf.Controls;
using UtilityHelperEx;
using System.Windows.Data;


namespace UtilityWpf.Controls.Chart
{
    using Mixins;
    using UtilityWpf.Controls.Chart.ViewModel;
    using static DependencyPropertyFactory<OxyChart>;
    using static UtilityWpf.Controls.Chart.ViewModel.MultiTimeModel;

    public class OxyChart : Controlx, IItemsSource
    {
        public static readonly DependencyProperty ItemsSourceProperty = Register(nameof(ItemsSource), a => a.Observer<IEnumerable>());
        public static readonly DependencyProperty IdProperty = Register(nameof(IdKey), a => a.Observer<string>(nameof(IdKey)));
        public static readonly DependencyProperty DataKeyProperty = Register(nameof(DataKey), a => a.Observer<string>(nameof(DataKey)));
        public static readonly DependencyProperty DataConverterProperty = Register(nameof(DataConverter), a => a.Observer<IValueConverter>());

        static OxyChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChart), new FrameworkPropertyMetadata(typeof(OxyChart)));
        }

        public OxyChart()
        {
            ISubject<MultiTimeModel> modelSubject = new Subject<MultiTimeModel>();

            var modelChanges = this
                .Control<PlotView>()
                .Subscribe(plotView =>
                {
                    plotView.Model ??= new PlotModel();
                    var model = new MultiTimeModel(plotView.Model);
                    modelSubject.OnNext(model);
                });

            modelSubject
                .CombineLatest(
                this.Observable<IEnumerable>()
                .WhereNotNull()
                .Select(cc =>
                {
                    return cc.MakeObservable().Select(a => cc);
                }),
                this.Observable<string>(nameof(DataKey)),
                this.Observable<string>(nameof(DataConverter)),
                 this.Observable<string>(nameof(IdKey)).Where(id => id != null))
                .ObserveOnDispatcher()
                .Subscribe(combination =>
                {
                    Combine(combination.First, combination.Second, combination.Third, combination.Fourth, combination.Fifth);
                });

            static void Combine(MultiTimeModel model, IObservable<IEnumerable> items, string dataKey, string converter, string idKey)
            {
                if (items == default(IObservable<string>))
                    model.Filter(null);
                else
                    items
                        .ObserveOnDispatcher()
                        .Subscribe(collection =>
                        {
                            HashSet<string> ids = GetIds(model, dataKey, converter, idKey, collection);

                            model.Filter(ids);
                            var colors = model.SelectColors();
                        }, e =>
                        {
                        },
                    () =>
                    { });

                static HashSet<string> GetIds(MultiTimeModel model, string key, string converter, string idKey, IEnumerable collection)
                {
                    // var itt = ItemsSource.Cast<object>().Select(o => o.GetType().GetProperty(IdProperty).GetValue(o).ToString());
                    HashSet<string> ids = new HashSet<string>();
                    foreach (var item in collection)
                    {
                        Color color = (Color)item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(Color))?.GetValue(item);
                        var aa = item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(TimeSpan));

                        TimeSpan? timeSpan = (TimeSpan?)aa?.GetValue(item);

                        var id = item.GetType().GetProperty(idKey).GetValue(item).ToString();
                        ids.Add(id);

                        if (color != default)
                        {
                            model.OnNext(new KeyValuePair<string, Color>(id, color));
                        }
                        if (timeSpan != default)
                        {
                            model.OnNext(new KeyValuePair<string, TimeSpan>(id, timeSpan.Value));
                        }

                        IEnumerable<DateTimePoint> data;
                        if (converter != null)
                        {
                            data = item.GetType().GetProperties().FirstOrDefault(a => a.Name == key)?.GetValue(item) as IEnumerable<DateTimePoint>;
                        }
                        else if (key != null)
                        {
                            data = item.GetType().GetProperties().FirstOrDefault(a => a.Name == key)?.GetValue(item) as IEnumerable<DateTimePoint>;
                        }
                        else
                        {
                            continue;
                            //return ids;
                        }


                        data
                            .MakeObservable()
                            .Select(a => new KeyValuePair<string, (DateTime dt, double d)>(id, (a.DateTime, a.Value))).Buffer(TimeSpan.FromSeconds(0.5))
                            .Where(a => a.Count > 0)
                            .Subscribe(data =>
                            {
                                model.OnNext(data);
                            });
                    }

                    return ids;
                }
            }
        }






        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string DataKey
        {
            get { return (string)GetValue(DataKeyProperty); }
            set { SetValue(DataKeyProperty, value); }
        }
        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public string IdKey
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
    }
}