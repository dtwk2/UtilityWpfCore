using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for StructDataGridUserControl.xaml
    /// </summary>
    public partial class StructDataGridUserControl : UserControl
    {
        public StructDataGridUserControl()
        {
            InitializeComponent();
            this.DataContext = Enumerable.Range(0, 4).Select(_ => new Point(_, _ * 3)).ToList();
        }
    }
}