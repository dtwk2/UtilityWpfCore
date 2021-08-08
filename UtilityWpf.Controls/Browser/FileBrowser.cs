using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;


namespace UtilityWpf.Controls.Browser
{
    public class FileBrowser : PathBrowser
    {
        private readonly Subject<string> filterChanges = new Subject<string>();
        private readonly Subject<string> extensionChanges = new Subject<string>();

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(PathBrowser), new PropertyMetadata(null, FilterChanged));

        public static readonly DependencyProperty ExtensionProperty =
            DependencyProperty.Register("Extension", typeof(string), typeof(PathBrowser), new PropertyMetadata(null, ExtensionChanged));

        public FileBrowser()
        {
            _ = applyTemplateSubject.Select(a => ButtonOne?.ToClicks() ?? throw new NullReferenceException("ButtonOne is null"))
                .SelectMany(a => a)
                .CombineLatest(filterChanges.StartWith(Filter).DistinctUntilChanged(),
                    extensionChanges.StartWith(Extension).DistinctUntilChanged(),
                    (a, b, c) => OpenDialog(b, c))
                .Where(output => output.result ?? false)
                .ObserveOnDispatcher()
                .Select(output => output.path)
                .WhereNotNull()
                .Subscribe(textChanges.OnNext);
        }

        protected override (bool? result, string path) OpenDialog(string filter, string extension)
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

        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FileBrowser)?.filterChanges.OnNext((string)e.NewValue);
        }

        public string Extension
        {
            get => (string)GetValue(ExtensionProperty);
            set => SetValue(ExtensionProperty, value);
        }

        private static void ExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FileBrowser ?? throw new NullReferenceException("FileBrowser object is null")).extensionChanges.OnNext((string)e.NewValue);
        }
    }
}