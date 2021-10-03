using System.Windows.Controls;
using UtilityWpf.Demo.Common.Meta;

namespace UtilityWpf.Demo.Hybrid
{
    /// <summary>
    /// Interaction logic for FactoryLogUserControl.xaml
    /// </summary>
    public partial class FactoryLogUserControl : UserControl
    {
        public FactoryLogUserControl()
        {
            InitializeComponent();
            MainDataGrid.ItemsSource = Statics.Service<FactoryLogger>().Logs;
        }
    }
}
