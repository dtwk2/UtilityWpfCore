using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using UtilityWpf.Controls;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.View
{
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