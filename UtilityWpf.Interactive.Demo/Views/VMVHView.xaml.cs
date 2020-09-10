using System.Windows.Controls;
using UtilityWpf.Model;

namespace UtilityWpf.Interactive.Demo
{
    /// <summary>
    /// Interaction logic for VMVHView.xaml
    /// </summary>
    public partial class VMVHView : UserControl
    {
        public VMVHView()
        {
            InitializeComponent();

            InteractiveList2.Data = new ViewModelAssemblyModel().Collection.Result;
        }
    }
}