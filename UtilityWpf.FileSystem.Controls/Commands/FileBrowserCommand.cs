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
        private bool multiSelect;
        protected Subject<string> filterChanges = new();
        protected Subject<string> extensionChanges = new();
        protected Subject<bool> multiSelectChanges = new();

        public FileBrowserCommand() : base(Select(out var filterSubject, out var extensionSubject, out var multiSelect))
        {
            filterChanges.StartWith(Filter).Subscribe(filterSubject);
            extensionChanges.StartWith(Extension).Subscribe(extensionSubject);
            multiSelectChanges.StartWith(IsMultiSelect).Subscribe(multiSelect);
        }

        private static Func<IObservable<string>> Select(out ReplaySubject<string> filter, out ReplaySubject<string> extension, out ReplaySubject<bool> multiSelectChanges)
        {
            filter = new(1);
            extension = new(1);
            multiSelectChanges = new(1);

            var obs = filter.CombineLatest(extension, multiSelectChanges, (b, c, d) => OpenDialog(b, c, d))
                .Take(1)
            .Where(output => output.result ?? false)
            .ObserveOnDispatcher()
            .SelectMany(output => output.path.ToObservable())
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
        public bool IsMultiSelect
{
            get => multiSelect; set { multiSelect = value; multiSelectChanges.OnNext(value); }
        }

        protected static (bool? result, string[] path) OpenDialog(string filter, string extension, bool isMultiSelect)
        {
            OpenFileDialog dlg = new();
            dlg.Multiselect = isMultiSelect;
            //  dlg.DefaultExt = ".png";
            //   dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (extension != null)
                dlg.DefaultExt = extension.StartsWith(".") ? extension : "." + extension;
            if (filter != null)
                dlg.Filter = filter;

            bool? result = dlg.ShowDialog();
         
            // if dialog closed dlg.FileName may become inaccessible; hence check for result equal to true
            return result == true ? (result, dlg.FileNames) : (result, null);
        }
    }
}