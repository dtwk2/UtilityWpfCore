using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace UtilityWpf
{
    public static class ObservableHelper
    {
        /// <summary>
        /// https://stackoverflow.com/questions/28853030/iobservable-ignore-new-elements-for-a-span-of-time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sampleDuration"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<T> SampleFirst<T>(
    this IObservable<T> source,
    TimeSpan sampleDuration,
    IScheduler scheduler = null)
        {
            scheduler ??= Scheduler.Default;
            return source.Publish(ps =>
                ps.Window(() => ps.Delay(sampleDuration, scheduler))
                  .SelectMany(x => x.Take(1)));
        }

        //public static IObservable<RoutedEventArgs> SelectLoads(this FrameworkElement element) =>
        //    Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => element.Loaded += a, a => element.Loaded -= a)
        //    .Select(a => a.EventArgs);

        /// James World
        /// http://www.zerobugbuild.com/?p=323
        /// The events should be output at a maximum rate specified by a TimeSpan, but otherwise as soon as possible.
        public static IObservable<T> Pace<T>(this IObservable<T> source, TimeSpan rate)
        {
            var paced = source.Select(i => Observable.Empty<T>()

                                      .Delay(rate)
                                      .StartWith(i)).Concat();

            return paced;
        }
    }
}