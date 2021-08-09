using System.Linq;
using System.Windows;
using UtilityWpf.Demo.Animation.View;
using UtilityWpf.Demo.View;
using UtilityWpf.Demo.Panels.View;

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
            var assemblies = UtilityHelper.AssemblyHelper.GetNonSystemAssembliesInCurrentDomain();
            AssemblyComboBox.ItemsSource = assemblies.Where(a => a.FullName.Contains(".View"));
        }

    }
}