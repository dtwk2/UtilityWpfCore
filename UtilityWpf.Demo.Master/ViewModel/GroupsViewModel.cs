using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using UtilityWpf.Demo.Master.Infrastructure;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.View
{
    public class GroupsViewModel
    {
        public GroupsViewModel()
        {
            Data = new ObservableCollection<object> { new RowViewModel(), new NotesViewModel(), };
            ChangeCommand = ReactiveUI.ReactiveCommand.Create<object, Unit>((a) =>
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
        public ObservableCollection<object> Data { get; }

        public ICommand ChangeCommand { get; }

    }
}