using Splat;
using System.Windows.Controls;
using UtilityWpf.Abstract;
using UtilityWpf.Model;

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

        private async void Init()
        {
            Main_MasterDetailView.ItemsSource = await (Locator.Current.GetService<IViewModelAssemblyModel>()).Collection;
        }
    }
}