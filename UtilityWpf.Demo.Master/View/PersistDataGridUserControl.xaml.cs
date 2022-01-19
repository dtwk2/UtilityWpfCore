using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    /// <summary>
    /// Interaction logic for PersistListUserControl.xaml
    /// </summary>
    public partial class PersistDataGridUserControl : UserControl
    {
        public PersistDataGridUserControl()
        {
            InitializeComponent();
            //(this.DataContext as PersistListViewModel).Data = PersistBehavior.Items;
        }

        private void DragablzItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
