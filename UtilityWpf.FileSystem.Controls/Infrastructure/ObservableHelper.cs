using System;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf.Controls.FileSystem.Infrastructure
{
    public static class ObservableHelper
    {
        public static IObservable<string> SelectPathChanges(this PathBrowser pathBrowser)
        {
            return Observable.FromEventPattern<TextChangedRoutedEventHandler, TextChangedRoutedEventArgs>(
                   a => pathBrowser.TextChange += a,
                   a => pathBrowser.TextChange -= a)
                .Select(a => a.EventArgs.Text);
        }
    }
}
