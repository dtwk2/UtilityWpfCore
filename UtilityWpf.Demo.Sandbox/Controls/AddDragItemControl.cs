using Dragablz;
using UtilityWpf.Controls;

namespace UtilityWpf.Demo.View
{
    public class AddDragItemControl : MasterControl
    {

        public AddDragItemControl()
        {            
        }

        protected override void ExecuteAdd()
        {
            //if (Content is DragablzItemsControl itemsControl)
            //    itemsControl.AddToSource(parameter, AddLocationHint.Last);
            //else
            //    throw new System.Exception("dfsd    sdf");
            base.ExecuteAdd();
        }
    }
}