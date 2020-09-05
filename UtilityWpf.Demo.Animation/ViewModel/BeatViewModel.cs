using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf.DemoAnimation
{
    public class BeatViewModel : ReactiveObject
    {
        private double rate = 1d;
        private ObservableAsPropertyHelper<long> beat;

        public double Rate { get => rate; set => this.RaiseAndSetIfChanged(ref rate, value); }

        public long Beat => beat.Value;

        public BeatViewModel()
        {
            beat = this.WhenAnyValue(a => a.Rate).Where(_ => _ > 0).Select(_ => Observable.Interval(TimeSpan.FromSeconds(1d / _))).Switch().ToProperty(this, a => a.Beat);
        }
    }
}