using MaterialDesignExtensions.Controls;
using Microsoft.Xaml.Behaviors.Core;
using ReactiveUI;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UtilityWpf.Controls
{
    public class FileView : Control
    {
        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.Register("Directory", typeof(string), typeof(FileView), new PropertyMetadata(null));

        private OpenFileControl? openFileControl;
        private ContentControl? contentControl;

        static FileView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileView), new FrameworkPropertyMetadata(typeof(FileView)));
        }

        public override void OnApplyTemplate()
        {
            openFileControl = GetTemplateChild("OpenFileControl1") as OpenFileControl ?? throw new NullReferenceException("FileView object is null");
            contentControl = GetTemplateChild("ContentControl1") as ContentControl ?? throw new NullReferenceException("FileView object is null");

            this.WhenAnyValue(v => v.Directory)
                .Subscribe(directory =>
            {
                openFileControl.CurrentDirectory = directory;
            });

            openFileControl.FileSelected += OpenFileControl_FileSelected;

            Refresh = new ActionCommand(() =>
            {
                string directory = Directory;
                Directory = "c:\\";
                Directory = directory;
            });
        }

        public FileView()
        {
        }

        public string Directory
        {
            get => (string)GetValue(DirectoryProperty);
            set => SetValue(DirectoryProperty, value);
        }

        public ICommand Refresh
        {
            get => (ICommand)GetValue(RefreshProperty);
            set => SetValue(RefreshProperty, value);
        }

        // Using a DependencyProperty as the backing store for Refresh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshProperty =
            DependencyProperty.Register("Refresh", typeof(ICommand), typeof(FileView), new PropertyMetadata(null));

        //public Type Type
        //{
        //    get => (Type)GetValue(TypeProperty);
        //    set => SetValue(TypeProperty, value);
        //}

        protected virtual void OpenFileControl_FileSelected(object sender, RoutedEventArgs e)
        {
            if (e is not FileSelectedEventArgs fileSelectedEventArgs) return;
            if (contentControl == null) return;

            switch (System.IO.Path.GetExtension(fileSelectedEventArgs.File))
            {
                case ".json":
                    contentControl.Content = ConvertJson(fileSelectedEventArgs.File);
                    break;

                case ".csv":
                    contentControl.Content = ConvertCsv(fileSelectedEventArgs.File);
                    break;

                case ".png":
                    var bitmap = new ImageSourceConverter().ConvertFromString(fileSelectedEventArgs.File);
                    contentControl.Content = new Image { Source = (ImageSource)bitmap };
                    break;

                case ".pdf":
                    contentControl.Content = ConvertPdf(fileSelectedEventArgs.File);
                    break;
            }
        }

        protected virtual object ConvertCsv(string filePath)
        {
            throw new NotImplementedException();
        }

        protected virtual object ConvertJson(string filePath)
        {
            var jsonView = new TextBlock { Text = File.ReadAllText(filePath) };
            return jsonView;
        }

        protected virtual object ConvertPdf(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}