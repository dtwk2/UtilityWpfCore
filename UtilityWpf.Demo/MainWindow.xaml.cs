using System.Linq;
using System.Windows;
using UtilityWpf.Demo.View.Animation;
using UtilityWpf.Demo.View;
using UtilityWpf.Demo.View.Panels;
using UtilityWpf.Controls;

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
            this.AddChild(new ViewsExDetailControl(new[] { c, a, b}));
        }

    }
}