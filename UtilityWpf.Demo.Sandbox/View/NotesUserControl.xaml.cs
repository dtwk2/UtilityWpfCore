using Dragablz;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for DragUserControl.xaml
    /// </summary>
    public partial class NotesUserControl : UserControl
    {
        private object[] _order;

        public NotesUserControl()
        {
            InitializeComponent();
        }


        private void MasterNotesControl_Change(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}