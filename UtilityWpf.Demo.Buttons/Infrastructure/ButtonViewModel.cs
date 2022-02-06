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

#pragma warning disable CS8618
        public ButtonViewModel()
#pragma warning restore CS8618
        {
        }

        public ICommand Command { get; init; }

        public string Header { get; init; }
    }
}