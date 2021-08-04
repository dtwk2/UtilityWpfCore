using Dragablz;
using HandyControl.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using System.Windows.Input;
using UtilityWpf.Controls;
using UtilityWpf.DemoApp.View;
using static UtilityWpf.Controls.MasterControl;
using Kaos.Collections;

namespace UtilityWpf.DemoApp.View
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
    }

    public class MainViewModel
    {

        public MainViewModel()
        {
            Rows = new ObservableCollection<RowViewModel>
            {
                new RowViewModel(),
                new RowViewModel(),
            };

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