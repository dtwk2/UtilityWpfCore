using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using UtilityWpf.Demo.Master.Infrastructure;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.View
{
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
}