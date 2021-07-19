using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using UtilityWpf;

namespace UtilityWpf
{
    public static class ObservableHelper
    {
        public static ReplaySubject<T> ToReplaySubject<T>(this IObservable<T> source, int save = 1)
        {
            var replaySubject = new ReplaySubject<T>(save);
            source.Subscribe(replaySubject);
            return replaySubject;
        }


    }
}