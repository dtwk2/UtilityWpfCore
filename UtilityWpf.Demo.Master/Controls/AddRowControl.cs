using System.Windows.Controls;
using UtilityWpf.Controls;
using UtilityWpf.Controls.Master;
using UtilityWpf.Demo.Master.Infrastructure;
using UtilityWpf.Demo.View;

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