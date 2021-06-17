using DynamicData;
using DynamicData.Operators;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace UtilityWpf.Model
{
    public class FilteredPaginatedModel<T> : ReactiveObject
    {
        private IPageResponse pageResponse;
        private ReadOnlyObservableCollection<T> pitems;

        public FilteredPaginatedModel(IObservable<IChangeSet<T>> obs, IObservable<PageRequest> request, IObservable<Func<T, bool>> filter)
        {
            obs
                  .Filter(filter)
                  .Page(request)
                           .Do(_ =>
                           {
                               PageResponse = ((IPageChangeSet<T>)_).Response;
                           })
                           .Bind(out pitems)
                           .DisposeMany()
                           .Subscribe();
        }

        public ReadOnlyObservableCollection<T> Items => pitems;
        public IPageResponse PageResponse { get => pageResponse; set => this.RaiseAndSetIfChanged(ref pageResponse, value); }

    }
}