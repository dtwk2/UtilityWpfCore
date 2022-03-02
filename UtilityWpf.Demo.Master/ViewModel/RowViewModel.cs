using Microsoft.Xaml.Behaviors.Core;
using Utility.Common.Enum;
using Utility.Common.EventArgs;

namespace UtilityWpf.Demo.Master.ViewModel
{
    public class RowViewModel : UtilityWpf.Demo.Common.ViewModel.RowBaseViewModel
    {
        public RowViewModel()
        {
            ChangeCommand = new ActionCommand((a) =>
            {
                switch (a)
                {
                    case CollectionItemEventArgs { EventType: EventType.Add }:
                        if (NewItem.MoveNext())
                            Data.Add(NewItem.Current as Common.ViewModel.ElementViewModel);
                        break;

                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            Data.Move(item.OldIndex, item.Index);
                        }
                        break;

                    default:
                        break;
                }
            });
        }
    }
}