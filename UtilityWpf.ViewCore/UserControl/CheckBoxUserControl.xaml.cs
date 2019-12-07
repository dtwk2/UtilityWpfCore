using System.Windows.Controls;

namespace UtilityWpf.View
{
    /// <summary>
    /// Interaction logic for RadioUserControl.xaml
    /// </summary>
    public partial class CheckBoxUserControl : UserControl
    {
        public CheckBoxUserControl()
        {
            InitializeComponent();
            usercontrol.DataContext = this;
        }
    }
}