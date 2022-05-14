using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace UtilityWpf.Helper
{
    public static class AnimationHelper
    {
        public static IObservable<EventArgs> SelectCompletions(this Storyboard storyboard) =>

            Observable
            .FromEventPattern<EventHandler, EventArgs>
            (a => storyboard.Completed += a, a => storyboard.Completed -= a)
            .Select(a => a.EventArgs);

        public static Task<EventPattern<object>> ToTask(this DoubleAnimation animation)
        {
            return Observable.FromEventPattern(a => animation.Completed += a, a => animation.Completed -= a).Take(1).ToTask();
        }

    }
}