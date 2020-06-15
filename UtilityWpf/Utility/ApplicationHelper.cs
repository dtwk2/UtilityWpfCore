using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace UtilityWpf
{
    public static class ApplicationHelper
    {
        public static IObservable<ExitEventArgs> GetExitEvents(this Application app)
        {
            return System.Reactive.Linq.Observable
        .FromEventPattern<ExitEventHandler, ExitEventArgs>(h => app.Exit += h, h => app.Exit -= h)
           .Select(_ => _.EventArgs);
        }
    }
}