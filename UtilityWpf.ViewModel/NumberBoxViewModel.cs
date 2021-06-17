using System.Reactive.Linq;
using UtilityWpf.Abstract;
using UtilityWpf.Attribute;

namespace UtilityWpf.ViewModel
{
    [ViewModel]
    public class NumberBoxViewModel : OutputService<int>
    {
        public NumberBoxViewModel(string title = "", int value = 1) : base(Observable.Return(value))
        {
            Title = title;
        }

        public string Title { get; }
        public int Minimum { get; } = 1;
        public int Maximum { get; } = 100;

    }
}