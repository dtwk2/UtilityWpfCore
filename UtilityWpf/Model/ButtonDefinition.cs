using UtilityWpf.Command;

namespace UtilityWpf.Model
{
    public class ButtonDefinition
    {

        public ButtonDefinition(object content, RelayCommand command)
        {
            Content = content;
            Command = command;
        }
        public object Content { get; }

        public RelayCommand Command { get; }
    }
}