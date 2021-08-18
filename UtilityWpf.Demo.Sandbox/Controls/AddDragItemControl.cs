using Dragablz;
using UtilityWpf.Controls;

namespace UtilityWpf.Demo.View
{
    public class AddDragItemControl : MasterControl
    {

        public AddDragItemControl()
        {
            
        }
        protected override void ExecuteAdd(object parameter)
        {
            //(Content as DragablzItemsControl).AddToSource(parameter, AddLocationHint.Last);
            base.ExecuteAdd(parameter);

        }


    }
}