using ReactiveUI;

namespace UtilityWpf.Demo.Buttons
{
    public class ToggleViewModel : ReactiveObject
    {
        private bool isChecked = true;

        public bool IsChecked
        {
            get => isChecked;
            set => this.RaiseAndSetIfChanged(ref isChecked, value);
        }
    }
}