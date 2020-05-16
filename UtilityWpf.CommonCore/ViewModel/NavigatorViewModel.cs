using DynamicData;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace UtilityWpf.ViewModel
{
    public class NavigatorVM:ReactiveObject
    {
        private int size;

        public ICommand NextCommand { get; } = ReactiveUI.ReactiveCommand.Create(() => new Unit());

        public ICommand PreviousCommand { get; } = ReactiveUI.ReactiveCommand.Create(() => new Unit());

        public int Size { get => size; set => this.RaiseAndSetIfChanged(ref size, value); }
    }

    public class NavigatorViewModel : NavigatorVM //, IOutputService<DynamicData.PageRequest>
    {
        private readonly ObservableAsPropertyHelper<DynamicData.PageRequest> output;

        public DynamicData.PageRequest Output => output.Value;

        public NavigatorViewModel(IObservable<int> currentPage, IObservable<int> pageSize)
        {
            pageSize.Subscribe(p=>Size=p);

            var obs = currentPage.DistinctUntilChanged().CombineLatest(this.WhenAnyValue(a=>a.Size), (a, b) =>
            new { page = a, size = b });

            output = (NextCommand as ReactiveCommand<Unit,Unit>).WithLatestFrom(obs, (a, b) => new PageRequest(b.page + 1, b.size)).Merge
   ((PreviousCommand as ReactiveCommand<Unit, Unit>).WithLatestFrom(obs, (a, b) =>
   new PageRequest(b.page - 1, b.size))).StartWith(new PageRequest(1, 25)).ToProperty(this, a=>a.Output);
        }
    }
}