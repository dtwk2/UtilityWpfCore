using System.Windows.Controls;
using UtilityWpf.ViewModel;

namespace UtilityWpf.Interactive.Demo
{
    /// <summary>
    /// Interaction logic for ViewModelAssemblyView.xaml
    /// </summary>
    public partial class ViewModelAssemblyView : UserControl
    {
        public ViewModelAssemblyView()
        {
            InitializeComponent();

            Init();
        }

        async void Init()
        {
            Main_MasterDetailView.ItemsSource = await new ViewModelAssemblyModel().Collection;
        }
    }
}
