using System.Windows.Input;

namespace UtilityWpf.Demo.Buttons.Infrastructure
{
    public class ButtonViewModel
    {

        public ButtonViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public ButtonViewModel()
        {
        }


        public ICommand Command { get; init; }

        public string Header { get; init; }

    }

}