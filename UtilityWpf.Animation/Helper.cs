using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Media.Animation;

namespace UtilityWpf.Animation
{
    public static class Helper
    {
        public static IObservable<EventArgs> SelectCompletions(this Storyboard storyboard) =>

     Observable
     .FromEventPattern<EventHandler, EventArgs>
     (a => storyboard.Completed += a, a => storyboard.Completed -= a)
     .Select(a => a.EventArgs);

        public static ReplaySubject<T> ToReplaySubject<T>(this IObservable<T> source, int save = 1)
        {
            var replaySubject = new ReplaySubject<T>(save);
            source.Subscribe(replaySubject);
            return replaySubject;
        }
    }
}
