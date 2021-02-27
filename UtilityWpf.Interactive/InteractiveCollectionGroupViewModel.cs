# nullable enable

using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using UtilityInterface.Generic;
using UtilityWpf.Interactive.Model;
using UtilityWpf.Model;

namespace UtilityWpf.Interactive
{
    public class InteractiveCollectionGroupViewModel<T, R> : InteractiveCollectionBase<object>, ICollectionViewModel<IObject<object>>, IDisposable
    {
        private readonly IDisposable disposable;

        public InteractiveCollectionGroupViewModel(
            IObservable<IChangeSet<T, R>> observable,
            IObservable<string?> groupKey,
            IObservable<Predicate<T>>? visiblefilter = null,
            IObservable<Predicate<T>>? enabledfilter = null,
            IObservable<Func<T, object>>? getkey = null,
            IObservable<bool>? deletable = null,
            IObservable<bool>? doubleClickToCheck = null,
            string? title = null)
        {
            observable.Subscribe(a =>
            {
            });

            disposable = observable
                 .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(s =>
                {
                    var so = new InteractiveObject<object>(s,
                        visible: visiblefilter?.Select(a => (bool?)a(s)),
                        enabled: enabledfilter?.Select(a => (bool?)a(s)),
                        removable: deletable,
                        doubleClickToCheck: doubleClickToCheck,
                        groupKeyName: groupKey,
                        id: getkey?.Select(a => a(s)));

                    this.ReactToChanges(so);

                    return so;
                })
                .GroupOnProperty(a => a.GroupKey)
                .Transform(s =>
               {
                   var ds = s.Cache.Connect().ToCollection();
                   var dis = s.Cache.Connect().Bind(out var coll).Subscribe();
                   var so = new InteractiveObject<object>(s.Key, true, collection: coll);
                   return (IObject<object>)so;
               })
             .Bind(out items)
             .DisposeMany()
             .Subscribe(
                a =>
                {
                    //foreach (var x in a.Select(a => new KeyValuePair<IObject<object>, ChangeReason>(a.Current, a.Reason)))
                    //    (Changes as Subject<KeyValuePair<IObject<object>, ChangeReason>>).OnNext(x);
                },
                ex =>
                {
                    //(errors as ISubject<Exception>).OnNext(ex);
                    //Console.WriteLine("Error in generic view model");
                },
                () => Console.WriteLine("observable completed"));

            //Title = title;
        }

        //public  ReadOnlyObservableCollection<InteractiveObject<object>> Items => items;

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}