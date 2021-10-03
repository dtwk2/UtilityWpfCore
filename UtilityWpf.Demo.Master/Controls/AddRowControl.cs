using System.Windows.Controls;
using UtilityWpf.Controls.Master;
using UtilityWpf.Demo.Master.ViewModel;
using RowViewModel = UtilityWpf.Demo.Master.ViewModel.RowViewModel;

namespace UtilityWpf.Demo.Master.Controls
{
    public class AddRowControl : MasterControl
    {
        protected override void ExecuteAdd()
        {
            ((Content as ListBox).DataContext as MainViewModel).Rows.Add(new RowViewModel());
        }
    }
}