using DynamicData;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace UtilityWpf.ViewModel
{

    public class NavigatorViewModel : NavigatorBaseViewModel //, IOutputService<DynamicData.PageRequest>
    {
        private readonly ObservableAsPropertyHelper<PageRequest> output;

        public NavigatorViewModel(IObservable<int> currentPage, IObservable<int> pageSize)
        {
            pageSize.Subscribe(p => Size = p);

            var obs = currentPage.DistinctUntilChanged().CombineLatest(this.WhenAnyValue(a => a.Size), (a, b) =>
              new { page = a, size = b });

            output = (NextCommand as ReactiveCommand<Unit, Unit>).WithLatestFrom(obs, (a, b) => new PageRequest(b.page + 1, b.size)).Merge
                ((PreviousCommand as ReactiveCommand<Unit, Unit>).WithLatestFrom(obs, (a, b) => new PageRequest(b.page - 1, b.size)))
                .StartWith(new PageRequest(1, 25)).ToProperty(this, a => a.Output);
        }

        public PageRequest Output => output.Value;
    }
}