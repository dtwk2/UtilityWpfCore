using Dragablz;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using System.Windows.Input;
using UtilityWpf.Controls;
using static UtilityWpf.Controls.MasterControl;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

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

    class DeleteAdorner : Adorner
    {
        public DeleteAdorner(UIElement adornerElement) : base(adornerElement)
        { }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Blue, new Pen(Brushes.Red, 1),
            new Rect(new Point(10, DesiredSize.Height-40), new Size(DesiredSize.Width-20, 50)));
            base.OnRender(drawingContext);
        }
    }
    public class MainViewModel
    {
        public MainViewModel()
        {
            Rows = new ObservableCollection<RowViewModel> { new RowViewModel(), new RowViewModel(), };
            ChangeCommand = ReactiveUI.ReactiveCommand.Create<object, Unit>((a) =>
            {
                switch (a)
                {
                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            Rows.Move(item.OldIndex, item.Index);
                        }
                        break;
                    default:
                        break;
                }
                return Unit.Default;
            });
        }
        public ObservableCollection<RowViewModel> Rows { get; }

        public ICommand ChangeCommand { get; }

    }

    public class AddDragItemControl : MasterControl
    {

        public AddDragItemControl()
        {
            
        }
        protected override void ExecuteAdd(object parameter)
        {
            (Content as DragablzItemsControl).AddToSource(parameter, AddLocationHint.Last);
            base.ExecuteAdd(parameter);

        }


    }

    public class AddRowControl : MasterControl
    {
        protected override void ExecuteAdd(object parameter)
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

            ChangeCommand = ReactiveUI.ReactiveCommand.Create<MasterControl.EventArgs, Unit>((a) =>
            {
                switch (a)
                {
                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            Data.Move(item.OldIndex, item.Index);
                        }
                        break;
                    default:
                        break;
                }
                return Unit.Default;
            });
        }

        public ObservableCollection<int> Data { get; }

        public ICommand ChangeCommand { get; }

        public int NewItem => Data.Last() + 1;
    }
}