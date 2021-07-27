﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.Controls
{
    using Command;

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
    }
}