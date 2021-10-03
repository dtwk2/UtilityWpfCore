using Endless;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using Utility.Common.EventArgs;

namespace UtilityWpf.Demo.Common.ViewModel
{

    public class RowViewModel
    {
        private bool isReadOnly;
        private IEnumerator<ElementViewModel> newItem;

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

            ChangeCommand = ReactiveUI.ReactiveCommand.Create<CollectionEventArgs, Unit>((a) =>
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

        public IEnumerator NewItem => newItem ??= Get().Repeat().GetEnumerator();

        ElementViewModel Get()
        {
            return new(Data.LastOrDefault()?.Value ?? 0 + 1);
        }
    }
}


