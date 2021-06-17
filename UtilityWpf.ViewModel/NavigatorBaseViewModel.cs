using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace UtilityWpf.ViewModel

{
    public class NavigatorBaseViewModel : ReactiveObject
    {
        private int size;

        public ICommand NextCommand { get; } = ReactiveCommand.Create(() => new Unit());

        public ICommand PreviousCommand { get; } = ReactiveCommand.Create(() => new Unit());

        public int Size { get => size; set => this.RaiseAndSetIfChanged(ref size, value); }
    }
}