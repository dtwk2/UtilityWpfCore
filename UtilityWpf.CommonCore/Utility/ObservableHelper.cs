using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

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
    }
}
