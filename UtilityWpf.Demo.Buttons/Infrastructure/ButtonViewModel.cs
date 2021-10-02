using System.Windows.Input;

namespace UtilityWpf.Demo.Buttons
{
    public class ButtonViewModel
    {

        public ButtonViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public ICommand Command { get; init; }

        public string Header { get; init; }

    }

}