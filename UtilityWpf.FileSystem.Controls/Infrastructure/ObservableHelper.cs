using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityWpf.Controls.FileSystem.Infrastructure
{
    public static class ObservableHelper
    {
        public static IObservable<string> SelectPathChanges(this PathBrowser pathBrowser)
        {
            return Observable.FromEventPattern<PathBrowser.TextChangeRoutedEventHandler, PathBrowser.TextRoutedEventArgs>(
                   a => pathBrowser.TextChange += a,
                   a => pathBrowser.TextChange -= a)
                .Select(a => a.EventArgs.Text);
        }
    }
}
