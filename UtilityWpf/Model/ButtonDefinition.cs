using UtilityWpf.Command;

namespace UtilityWpf.Model
{
    public class ButtonDefinition
    {
        public object Content { get; set; }
        public RelayCommand Command { get; set; }
    }
}