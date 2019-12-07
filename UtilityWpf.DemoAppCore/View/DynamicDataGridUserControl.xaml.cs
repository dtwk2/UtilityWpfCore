using System.Linq;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for DynamicDataGridUserControl.xaml
    /// </summary>
    public partial class DynamicDataGridUserControl : UserControl
    {
        public DynamicDataGridUserControl()
        {
            InitializeComponent();
            this.DataContext = Enumerable.Range(0, 4).Select(_ => Enumerable.Range(0, 3).ToDictionary(a_ => a_.ToString(), a_ => (a_ * 3).ToString()));
        }
    }
}