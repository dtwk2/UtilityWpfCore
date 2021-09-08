using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace UtilityWpf.Demo.Common.ViewModel
{

    public class ButtonViewModel
    {

        public ButtonViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public ICommand Command { get; set; }

        public string Header { get; set; }

    }

    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>
            {
                new("1", ReactiveCommand.Create(()=>{ })),
                new("2", ReactiveCommand.Create(()=>{ })),
                new("3", ReactiveCommand.Create(()=>{ })),              
            };
        }

        public ObservableCollection<ButtonViewModel> Data { get; } 

    }
}
