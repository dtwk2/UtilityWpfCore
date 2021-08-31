using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.Demo.FileSystem
{
    /// <summary>
    /// Interaction logic for FolderBrowserView.xaml
    /// </summary>
    public partial class FolderBrowserView : UserControl
    {
        public FolderBrowserView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void FolderBrowser1_OnTextChange(object sender, Controls.FileSystem.PathBrowser.TextRoutedEventArgs e)
        {

        }
    }
}
