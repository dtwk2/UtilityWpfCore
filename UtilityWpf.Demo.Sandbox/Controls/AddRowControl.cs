using System.Windows.Controls;
using UtilityWpf.Controls;

namespace UtilityWpf.Demo.View
{
    public class AddRowControl : MasterControl
    {
        protected override void ExecuteAdd(object parameter)
        {
            ((Content as ListBox).DataContext as MainViewModel).Rows.Add(new RowViewModel());
        }
    }
}