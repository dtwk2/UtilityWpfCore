using System.Linq;
using System.Windows;
using UtilityWpf.Demo.View.Animation;
using UtilityWpf.Demo.View;
using UtilityWpf.Demo.View.Panels;
using UtilityWpf.Controls;
using UtilityWpf.Demo.FileSystem;
using UtilityWpf.Controls.Master;
using UtilityWpf.Demo.Hybrid;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var a = typeof(BarUserControl);
            var b = typeof(CornerPanelView);
            var c = typeof(AdornerUserControl);
            var d = typeof(FileBrowserView);
            var e = typeof(MasterListUserControl);
            var f = typeof(MeasurementsUserControl);
            var g = typeof(UtilityWpf.Demo.Dragablz.NotesUserControl);
            this.AddChild(new ViewsExDetailControl(new[] { c, a, b, d, e, f, g}));
        }

    }
}