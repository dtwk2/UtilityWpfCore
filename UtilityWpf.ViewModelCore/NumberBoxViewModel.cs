//using Reactive.Bindings;

namespace UtilityWpf.ViewModel
{
    public class NumberBoxViewModel //: OutputService<int>
    {
        public string Title { get; }
        public int Minimum { get; } = 1;
        public int Maximum { get; } = 100;

        public NumberBoxViewModel(string title = "", int value = 1) //: base(new ReactiveProperty<int>(value))
        {
            Title = title;
        }
    }
}