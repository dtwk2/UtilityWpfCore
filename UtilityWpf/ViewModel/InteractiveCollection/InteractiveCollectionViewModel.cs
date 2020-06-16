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
    public class InteractiveCollectionViewModel<T, R> : InteractiveCollectionBase<T>, ICollectionViewModel<IObject<T>>, IDisposable
    {
        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable,
            IObservable<Predicate<T>> visiblefilter,
            IObservable<Predicate<T>> enabledfilter,
            Func<T, IConvertible> getkey = null, string title = null)
        {
            disposable = observable
                 .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(s =>
                {
                    var so = new SHDObject<T>(s, visiblefilter, enabledfilter, getkey?.Invoke(s));
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
                    (Errors as ISubject<Exception>).OnNext(ex);
                    Console.WriteLine("Error in generic view model");
                },
                () => Console.WriteLine("observable completed"));

            Title = title;
        }

        private IDisposable disposable;

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable, 
            IObservable<T> visible = null,
            IObservable<T> disable = null, 
            Func<T, IConvertible> getkey = null, 
            string title = null, 
            bool isReadonly = false)
        {
            disposable = observable

                .Transform(s =>
                {
                    var readableObservable = disable?.Scan(new List<T>(), (a, b) => { a.Add(b); return a; }).Select(_ => { Predicate<T> f = a => !_.Any(b => b.Equals(a)); return f; });
                    var visibleObservable = visible?.Scan(new List<T>(), (a, b) => { a.Add(b); return a; }).Select(_ => { Predicate<T> f = a => !_.Any(b => b.Equals(a)); return f; });
                    var so = new SHDObject<T>(s, visibleObservable, readableObservable, getkey?.Invoke(s))
                    {
                        IsReadOnly = isReadonly
                    };
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
                    (Errors as ISubject<Exception>).OnNext(ex);
                    Console.WriteLine("Error in generic view model");
                },
                () =>
                Console.WriteLine("observable completed"));

            Title = title;
        }

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable,
            string childrenpath,
            IObservable<bool> ischecked,
            IObservable<bool> expand,
            Func<T, IConvertible> getkey = null,
            string title = null)
        {
            disposable = observable
     
                .Transform(s =>
                {
                    var so = new SEObject<T>(s, childrenpath, ischecked, expand, getkey?.Invoke(s));

                    so.OnPropertyChangeWithSource<SEObject<T>, KeyValuePair<T, InteractionArgs>>(nameof(SEObject<T>.ChildChanged)).Subscribe(_ =>
                    {
                        ChildSubject.OnNext(_.Item2);
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
                    (x as SEObject<T>).IsChecked = _;
            });

            Title = title;
        }

       
        public void Dispose()
        {
            disposable.Dispose();
        }
    }

}