using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using UtilityWpf.Controls;
using static UtilityWpf.Controls.MasterControl;
using Endless;
namespace UtilityWpf.Demo.Sandbox.ViewModel
{
    public class ElementViewModel
    {
        public ElementViewModel(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }

    public class RowViewModel
    {
        public RowViewModel()
        {
            Data = new ObservableCollection<ElementViewModel>
            {
                new(1),
                new(2),
                new(3)
            };

            ChangeCommand = ReactiveUI.ReactiveCommand.Create<EventArgs, Unit>((a) =>
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

        public ICommand ChangeCommand { get; }

        public IEnumerator NewItem => Get().Repeat().GetEnumerator();

        ElementViewModel Get()
        {
            return new(Data.LastOrDefault()?.Value ?? 0 + 1);
        }
    }
}