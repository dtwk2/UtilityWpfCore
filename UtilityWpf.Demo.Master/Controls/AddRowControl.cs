using System.Windows.Controls;
using UtilityWpf.Controls;
using UtilityWpf.Demo.Sandbox.ViewModel;

namespace UtilityWpf.Demo.View
{
    public class AddRowControl : MasterControl
    {
        protected override void ExecuteAdd()
        {
            ((Content as ListBox).DataContext as MainViewModel).Rows.Add(new RowViewModel());
        }
    }
}