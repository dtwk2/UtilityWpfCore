using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.Controls
{
    using System;
    using UtilityWpf.Property;

    public class FolderOpenControl : Control
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(FolderOpenControl), new PropertyMetadata(null, PathChanged));

        public static readonly DependencyProperty FolderOpenCommandProperty = DependencyProperty.Register("FolderOpenCommand", typeof(ICommand), typeof(FolderOpenControl));

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        static FolderOpenControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderOpenControl), new FrameworkPropertyMetadata(typeof(FolderOpenControl)));
        }

        public FolderOpenControl()
        {
            var foc = new FolderOpenCommand();
            this.SetValue(FolderOpenCommandProperty, foc);

            foc.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Directory")
                    this.Dispatcher.InvokeAsync(() => Path = foc.Directory,
                                   System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            };
        }

        class FolderOpenCommand : NPC, ICommand
        {
            private string directory;

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public string Directory
            {
                get { return directory; }
                set { OnPropertyChanged(ref directory, value); }
            }

            public void Execute(object parameter)
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        Directory = fbd.SelectedPath;
                }
            }
        }
    }
}