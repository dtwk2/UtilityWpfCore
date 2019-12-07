using System;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf.ViewModel
{
    public class TimerSingleton
    {
        public IObservable<DateTime> Time { get; private set; }

        private static TimerSingleton instance;

        private TimerSingleton()
        {
            DateTime d = DateTime.Now;

            Time = Observable.Interval(TimeSpan.FromSeconds(5))
                .Select(_ => d + TimeSpan.FromTicks(_))
                .Publish();
        }

        public static TimerSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimerSingleton();
                }
                return instance;
            }
        }
    }
}