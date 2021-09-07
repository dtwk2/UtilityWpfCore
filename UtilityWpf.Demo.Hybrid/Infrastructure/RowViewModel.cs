using System.Collections;
using System.Linq;
using System.Reactive;
using Endless;
using System.Windows.Input;
using UtilityWpf.TestData.Model;
using static UtilityWpf.Controls.Master.MasterControl;
using System.Collections.ObjectModel;

namespace UtilityWpf.Demo.Hybrid
{
    public class RowViewModel
    {
        private bool isReadOnly;
        public bool IsReadOnly
        {
            get => isReadOnly; set => isReadOnly = value;
        }

        public string Header { get; } = "RowViewModel";

        public RowViewModel()
        {
            Data = new ObservableCollection<ElementViewModel>
            {
                new(1),
                new(2),
                new(3)
            };

            ChangeCommand = ReactiveUI.ReactiveCommand.Create<Abstract.CollectionEventArgs, Unit>((a) =>
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

        public ObservableCollection<ElementViewModel> Data { get; }

        public ICommand ChangeCommand { get; init; }

        public IEnumerator NewItem => Get().Repeat().GetEnumerator();

        ElementViewModel Get()
        {
            return new(Data.LastOrDefault()?.Value ?? 0 + 1);
        }
    }
}


