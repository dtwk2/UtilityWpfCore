# nullable enable
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;
using UtilityWpf.Property;

namespace UtilityWpf.ViewModel
{

    public class InteractiveCollectionGroupViewModel<T, R> : InteractiveCollectionBase<object>, ICollectionViewModel<IObject<object>>, IDisposable
    {
        private readonly IDisposable disposable;

        public InteractiveCollectionGroupViewModel(IObservable<IChangeSet<T, R>> observable,
            IObservable<string?> groupKey,
            IObservable<Predicate<T>>? visiblefilter = null,
            IObservable<Predicate<T>>? enabledfilter = null,
            IObservable<Func<T, object>>? getkey = null,
            IObservable<bool>? deletable = null,
            IObservable<bool>? doubleClickToCheck = null,
            string? title = null)
        {
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
                    //this.ReactToChanges(so);
                    return so;
                })
                .Group(a => a.GroupKey)
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


    public class InteractiveCollectionViewModel<T, R> : InteractiveCollectionBase<T>, ICollectionViewModel<IObject<T>>, IDisposable
    {
        private readonly IDisposable disposable;

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable,
            IObservable<Predicate<T>>? visiblefilter = null,
            IObservable<Predicate<T>>? enabledfilter = null,
            IObservable<Func<T, object>>? getkey = null,
            IObservable<bool>? deletable = null,
            IObservable<bool>? doubleClickToCheck = null,
            string? title = null)
        {
            disposable = observable
                 .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(s =>
                {
                    var so = new InteractiveObject<T>(s,
                        visible: visiblefilter?.Select(a => (bool?)a(s)),
                        enabled: enabledfilter?.Select(a => (bool?)a(s)),
                        removable: deletable,
                        doubleClickToCheck: doubleClickToCheck,
                        id: getkey?.Select(a => a(s)));
                    this.ReactToChanges(so);
                    return (IObject<T>)so;
                })
                  .Bind(out items)
                  .DisposeMany()
                .Subscribe(
                _ =>
                {
                    foreach (var x in _.Select(a => new KeyValuePair<IObject<T>, ChangeReason>(a.Current, a.Reason)))
                        (Changes as Subject<KeyValuePair<IObject<T>, ChangeReason>>).OnNext(x);
                },
                ex =>
                {
                    (errors as ISubject<Exception>).OnNext(ex);
                    Console.WriteLine("Error in generic view model");
                },
                () => Console.WriteLine("observable completed"));

            Title = title;
        }





        //public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable, 
        //    IObservable<T> visible = null,
        //    IObservable<T> disable = null, 
        //    Func<T, IConvertible> getkey = null, 
        //    string title = null, 
        //    bool isReadonly = false):this(observable, visible.Select(a=> new Predicate<T>()), 
        //{
        //    disposable = observable

        //        .Transform(s =>
        //        {
        //            var readableObservable = disable?.Scan(new List<T>(), (a, b) => { a.Add(b); return a; }).Select(_ => { bool f(T a) => !_.Any(b => b.Equals(a)); return (Predicate<T>)f; });
        //            var visibleObservable = visible?.Scan(new List<T>(), (a, b) => { a.Add(b); return a; }).Select(_ => { bool f(T a) => !_.Any(b => b.Equals(a)); return (Predicate<T>)f; });
        //            var so = new InteractiveObject<T>(s, visibleObservable, readableObservable, getkey?.Invoke(s))
        //            {
        //                IsReadOnly = isReadonly
        //            };
        //            this.ReactToChanges(so);
        //            return (IObject<T>)so;
        //        })
        //         .Bind(out items)
        //         .DisposeMany()
        //         .Subscribe(
        //       _ =>
        //       {
        //           foreach (var x in _.Select(a => new KeyValuePair<IObject<T>, ChangeReason>(a.Current, a.Reason)))
        //               (Changes as Subject<KeyValuePair<IObject<T>, ChangeReason>>).OnNext(x);
        //       },
        //        ex =>
        //        {
        //            (Errors as ISubject<Exception>).OnNext(ex);
        //            Console.WriteLine("Error in generic view model");
        //        },
        //        () =>
        //        Console.WriteLine("observable completed"));

        //    Title = title;
        //}

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable,
            string childrenpath,
            IObservable<bool> ischecked,
            IObservable<bool> expand,
            Func<T, IConvertible>? getkey = null,
            string? title = null)
        {
            disposable = observable

                .Transform(s =>
                {
                    var so = new ChildInteractiveObject<T>(s, childrenpath, ischecked, expand, getkey?.Invoke(s));

                    so.OnPropertyChangeWithSource<ChildInteractiveObject<T>, KeyValuePair<T, InteractionArgs>>(nameof(ChildInteractiveObject<T>.ChildChanged)).Subscribe(_ =>
                    {
                        childSubject.OnNext(_.Item2);
                    });

                    this.ReactToChanges(so);
                    return (IObject<T>)so;
                })
                  .Bind(out items)
              .DisposeMany()
                .Subscribe(
                _ =>
                {
                    foreach (var x in _.Select(a => new KeyValuePair<IObject<T>, ChangeReason>(a.Current, a.Reason)))
                        (Changes as Subject<KeyValuePair<IObject<T>, ChangeReason>>).OnNext(x);
                },
                ex => Console.WriteLine("Error in generic view model"),
                () => Console.WriteLine("observable completed"));

            ischecked.DelaySubscription(TimeSpan.FromSeconds(0.5)).Take(1).Subscribe(_ =>
            {
                foreach (var x in items)
                    (x as ChildInteractiveObject<T>).IsChecked = _;
            });

            Title = title;
        }


        public void Dispose()
        {
            disposable.Dispose();
        }
    }

}