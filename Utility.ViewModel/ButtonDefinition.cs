using System.Windows.Input;

namespace UtilityWpf.Model
{
    public class ButtonDefinition
    {

        public ButtonDefinition(object content, ICommand command)
        {
            Content = content;
            Command = command;
        }
        public object Content { get; }

        public ICommand Command { get; }
    }
}