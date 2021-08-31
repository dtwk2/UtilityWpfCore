using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using ReactiveUI;

namespace UtilityWpf.Controls.FileSystem
{
    public class FolderBrowser : PathBrowser
    {
        public static readonly DependencyProperty IsFolderPickerProperty =            DependencyProperty.Register(nameof(IsFolderPicker), typeof(bool), typeof(FolderBrowser), new PropertyMetadata(true));

        public FolderBrowser()
        {
            _ = applyTemplateSubject.SelectMany(a => (ButtonOne ?? throw new NullReferenceException("ButtonOne is null")).ToClicks())
                .Select(a => OpenDialog(string.Empty, string.Empty))
                .Where(output => output.result ?? false)
                .ObserveOnDispatcher()
                .Select(output => output.path)
                .WhereNotNull()
                .Subscribe(textChanges.OnNext);
        }

        #region properties
        public bool IsFolderPicker
        {
            get => (bool)GetValue(IsFolderPickerProperty);
            set => SetValue(IsFolderPickerProperty, value);
        }
        #endregion properties

        protected override (bool? result, string path) OpenDialog(string filter, string extension)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                return result == DialogResult.OK ? (true, dialog.SelectedPath) : (false, null);
            }
        }
    }
}