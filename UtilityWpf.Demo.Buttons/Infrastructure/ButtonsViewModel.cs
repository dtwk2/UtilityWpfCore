using System.Collections.ObjectModel;
using ReactiveUI;
using Microsoft.Xaml.Behaviors.Core;

namespace UtilityWpf.Demo.Buttons
{
    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>
            {
                new("1", new ActionCommand(()=>{ })),
                new("2", ReactiveCommand.Create(()=>{ })),
                new("3", ReactiveCommand.Create(()=>{ })),
            };
        }

        public ObservableCollection<ButtonViewModel> Data { get; }

    }

}