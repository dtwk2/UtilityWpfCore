using Microsoft.Xaml.Behaviors.Core;
using ReactiveUI;
using System.Collections.ObjectModel;

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