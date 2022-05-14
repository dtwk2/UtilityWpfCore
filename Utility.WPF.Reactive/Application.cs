using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace UtilityWpf.Helper
{
    internal static class Application
    {
        public static IObservable<ExitEventArgs> GetExitEvents(this Application app) =>
            Observable
            .FromEventPattern<ExitEventHandler, ExitEventArgs>(h => app.Exit += h, h => app.Exit -= h)
           .Select(a => a.EventArgs);
    }
}