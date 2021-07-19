using Dragablz;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using UtilityWpf.Controls;

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for DragUserControl.xaml
    /// </summary>
    public partial class DragUserControl : UserControl
    {
        private object[] _order;

        public DragUserControl()
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
    }

    public class MainViewModel
    {
        public ObservableCollection<RowViewModel> Rows { get; }

        public MainViewModel()
        {
            Rows = new ObservableCollection<RowViewModel>
            {
                new RowViewModel(),
                new RowViewModel(),
            };
        }
    }

    public class AddDragItemControl : AddControl
    {
        public override void ExecuteAdd(object parameter)
        {
            (Content as DragablzItemsControl).AddToSource(parameter, AddLocationHint.Last);
        }
    }

    public class AddDragRowControl : AddControl
    {
        public override void ExecuteAdd(object parameter)
        {
            ((Content as ListBox).DataContext as MainViewModel).Rows.Add(new RowViewModel());
        }
    }

    public class RowViewModel
    {
        public RowViewModel()
        {
            Data = new ObservableCollection<int>
            {
                1,
                2,
                3
            };
        }

        public IEnumerable<int> Data { get; }

        public int NewItem => Data.Last() + 1;
    }
}