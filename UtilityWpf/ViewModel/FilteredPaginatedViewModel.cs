using DynamicData;
using DynamicData.Operators;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace UtilityWpf.ViewModel
{
    public class FilteredPaginatedViewModel<T> :ReactiveObject //: AbstractNotifyPropertyChanged
    {
        private object lck = new object();
        private IPageResponse pageResponse;
        private ReadOnlyObservableCollection<T> pitems;

        public ReadOnlyObservableCollection<T> Items => pitems;
        public IPageResponse PageResponse { get => pageResponse; set => this.RaiseAndSetIfChanged(ref pageResponse, value); }


        public FilteredPaginatedViewModel(IObservable<IChangeSet<T>> obs, IObservable<PageRequest> request, IObservable<Func<T, bool>> filter, IScheduler s)
        {
            obs.Synchronize(lck)
                  .Filter(filter)
                  .Page(request)
                           .Do(_ =>
                           {
                               lock (lck)
                               {
                                   PageResponse = ((IPageChangeSet<T>)_).Response;
                               }
                           })
                           .Bind(out pitems)
                           .DisposeMany()
                           .Subscribe();
        }
    }
}