using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace UtilityWpf.ViewModel
{
    public class ChartLine : ReactiveObject
    {
        public ObservableCollection<KeyValuePair<DateTime, double>> Items { get; set; } = new ObservableCollection<KeyValuePair<DateTime, double>>();

        public ChartLine(IObservable<KeyValuePair<DateTime, double>> series, IObservable<KeyValuePair<DateTime, double>> level, bool horizontal, IScheduler ds)
        {
            //this.Title = "Example 2";
            KeyValuePair<DateTime, double> xx = default(KeyValuePair<DateTime, double>);
            series.ObserveOn(ds).Subscribe(_ =>
            {
                //items.Add(_);
                if (Items.Count > 0)
                    if (horizontal)
                        xx = new KeyValuePair<DateTime, double>(_.Key, Items.Last().Value);
                    else
                        xx = new KeyValuePair<DateTime, double>(Items.Last().Key, _.Value);
                else
                    xx = _;

                Items.Add(xx);
            });

            level.ObserveOn(ds).Subscribe(_ =>
            {
                IEnumerable<KeyValuePair<DateTime, double>> xxl = null;

                if (horizontal)
                    xxl = Items.Select(j_ => new KeyValuePair<DateTime, double>(j_.Key, _.Value));
                else
                    xxl = Items.Select(j_ => new KeyValuePair<DateTime, double>(_.Key, j_.Value));

                Items = new ObservableCollection<KeyValuePair<DateTime, double>>(xxl);
                this.RaisePropertyChanged(nameof(Items));
            });
        }
    }
}

//        {
//            TradesVM = new CollectionViewModel<Trade>(ts.Trades, dispatcherservice.UI);
//            NotifyChanged(nameof(TradesVM));
//        });