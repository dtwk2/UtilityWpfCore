using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Win32;
using ReactiveUI;

namespace UtilityWpf.Controls.FileSystem
{
    public class FileBrowserCommand : BrowserCommand
    {
        private string filter;
        private string extension;
        protected Subject<string> filterChanges = new();
        protected Subject<string> extensionChanges = new();


        public FileBrowserCommand() : base(Select(out var filterSubject, out var extensionSubject))
        {
            filterChanges.StartWith(Filter).Subscribe(filterSubject);
            extensionChanges.StartWith(Extension).Subscribe(extensionSubject);
        }

        private static Func<IObservable<string>> Select(out ReplaySubject<string> filter, out ReplaySubject<string> extension)
        {
            filter = new(1);
            extension = new(1);

            var obs = filter.DistinctUntilChanged().WithLatestFrom(
                extension.DistinctUntilChanged(),
                (b, c) => OpenDialog(b, c))
                .Take(1)
            .Where(output => output.result ?? false)
            .ObserveOnDispatcher()
            .Select(output => output.path)
            .WhereNotNull();
            return new Func<IObservable<string>>(() => obs);

        }

        public string Filter
        {
            get => filter; set { filter = value; filterChanges.OnNext(value); }
        }
        public string Extension
        {
            get => extension; set { extension = value; extensionChanges.OnNext(value); }
        }

        protected static (bool? result, string path) OpenDialog(string filter, string extension)
        {
            OpenFileDialog dlg = new();

            //  dlg.DefaultExt = ".png";
            //   dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (extension != null)
                dlg.DefaultExt = extension.StartsWith(".") ? extension : "." + extension;
            if (filter != null)
                dlg.Filter = filter;

            bool? result = dlg.ShowDialog();

            // if dialog closed dlg.FileName may become inaccessible; hence check for result equal to true
            return result == true ? (result, dlg.FileName) : (result, null);
        }
    }
}