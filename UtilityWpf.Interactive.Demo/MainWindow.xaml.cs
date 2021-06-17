using Splat;
using System.Windows;

namespace UtilityWpf.Interactive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AssemblyViewModelViewHost.ViewModel = Locator.Current.GetService<ViewModelAssemblyViewModel>();
        }
    }
}