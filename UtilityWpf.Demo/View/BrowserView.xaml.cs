using System.Windows;
using System.Windows.Controls;
using Splat;
//using Utility.WPF.Controls.Browser;
using ReactiveUI;
using System;
using UtilityWpf.Controls.Browser;

namespace Utility.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for BrowserView.xaml
    /// </summary>
    public partial class BrowserView : UserControl
    {
        public BrowserView()
        {
            InitializeComponent();

            //var demo = Locator.Current.GetService<FolderDemoSelectorViewModel>();
            //this.FolderViewModelViewHost.ViewModel = demo;
 
            //this.FolderBrowser1.SetPath.Execute(demo.Path);
            //this.FolderBrowser1.SelectPathChanges().BindTo(demo, a => a.Path);

        }

        private void FolderBrowser1_OnTextChange(object sender, PathBrowser.TextRoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("Folder Change")));
        }

        private void FileBrowser_OnTextChange(object sender, PathBrowser.TextRoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("Path Change")));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            FolderBrowser1.SetPath.Execute("A path");
        }
    }
}
