using Dragablz;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for DragUserControl.xaml
    /// </summary>
    public partial class MasterListUserControl : UserControl
    {
        private object[] _order;

        public MasterListUserControl()
        {
            InitializeComponent();
            AddHandler(DragablzItem.DragStarted, new DragablzDragStartedEventHandler(ItemDragStarted), true);
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
        }

        private void ItemDragStarted(object sender, DragablzDragStartedEventArgs e)
        {
            //var item = e.DragablzItem.DataContext;
        }

        private void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            //var item = e.DragablzItem.DataContext;
            //System.Diagnostics.Trace.WriteLine($"User finished dragging item: {item}.");

            if (_order == null) return;

            System.Diagnostics.Trace.Write("Order is now: ");
            foreach (var i in _order)
            {
                System.Diagnostics.Trace.Write(i + " ");
            }
            System.Diagnostics.Trace.WriteLine("");
        }

        private void StackPositionMonitor_OnOrderChanged(object sender, OrderChangedEventArgs e)
        {
            _order = e.NewOrder;
        }

        private void DragablzItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        object number;
        double start = 0;
        private DeleteAdorner adorner;
        private AdornerLayer layer;

        private void HorizontalPositionMonitor_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (number != e.Item)
            {
                number = e.Item;
                start = e.Location.X;
            }
            if (System.Math.Abs(e.Location.X - start) > 100)
            {
                if (adorner == null)
                {
                    layer ??= AdornerLayer.GetAdornerLayer(this.MainListBox);
                    adorner = new DeleteAdorner(MainListBox);
                    layer.Add(adorner);
                }
                return;
            }
            adorner = null;
            layer ??= AdornerLayer.GetAdornerLayer(this.MainListBox);
            Adorner[] toRemoveArray = layer.GetAdorners(this.MainListBox);
            if (toRemoveArray != null)
                foreach (Adorner a in toRemoveArray)
                {
                    layer.Remove(a);
                }
        }
    }
}