using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;

namespace UtilityWpf.ViewModel
{
    public class InteractiveCollectionViewModel<T, R> : InteractiveCollectionBase<T>, ICollectionViewModel<IObject<T>>, IDisposable
    {
        //private ReadOnlyObservableCollection<SHDObject<T>> _items;

        public InteractiveCollectionViewModel(IObservableCache<T, R> observable,
            IObservable<Predicate<T>> visiblefilter,
            IObservable<Predicate<T>> enabledfilter,
            IScheduler scheduler, Func<T, IConvertible> getkey = null, string title = null)
        {
            disposable = observable.Connect()
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

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable, IObservable<T> visible = null, IObservable<T> disable = null, Func<T, IConvertible> getkey = null, string title = null, bool isReadonly = false)
        {
            //if (scheduler != null)
            //    observable = observable.ObserveOn(scheduler);

            disposable = observable

                .Transform(s =>
                {
                    var readableObservable = disable?.Scan(new List<T>(), (a, b) => { a.Add(b); return a; }).Select(_ => { Predicate<T> f = a => !_.Any(b => b.Equals(a)); return f; });
                    var visibleObservable = visible?.Scan(new List<T>(), (a, b) => { a.Add(b); return a; }).Select(_ => { Predicate<T> f = a => !_.Any(b => b.Equals(a)); return f; });
                    var so = new SHDObject<T>(s, visibleObservable, readableObservable, getkey?.Invoke(s));
                    so.IsReadOnly = isReadonly;
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

        //((System.Reactive.Subjects.ISubject<KeyValuePair<object, InteractionArgs>>) interactivecollection.Interactions).OnNext(new KeyValuePair<object, InteractionArgs>(_.d, new InteractionArgs { Interaction = Interaction.Include, Value = false }));

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable, string childrenpath, IObservable<bool> ischecked, IObservable<bool> expand, IScheduler scheduler, System.Windows.Threading.Dispatcher dispatcher, Func<T, IConvertible> getkey = null, string title = null)
        {
            disposable = observable
                 .ObserveOn(scheduler)
                .Transform(s =>
                {
                    var so = new SEObject<T>(s, childrenpath, ischecked, expand, dispatcher, getkey?.Invoke(s));

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

        public InteractiveCollectionViewModel(IObservable<IChangeSet<T, R>> observable, IObservable<Predicate<T>> invisiblefilter, IObservable<Predicate<T>> enabledfilter, IScheduler scheduler, Func<T, IConvertible> getkey = null, string title = null)
        {
            disposable = observable.ObserveOn(scheduler)
           .Transform(s =>
           {
               var so = new SHDObject<T>(s, invisiblefilter, enabledfilter, getkey?.Invoke(s));
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

        public void Dispose()
        {
            disposable.Dispose();
        }
    }

    //public static IObservable<IKey> GetOutput(this SelectableCollectionViewModel<IKey, string> Selections)
    //{
    //    return Selections.Output.BufferUntilInactive().Select(so => so.Object);
    //}
    //public static IObservable<T> GetSelectedObjectSteam<T, R>(this SelectableViewModel<T, R> si)
    //{
    //    //si.WhenValueChanged(t => t.SelectedItem).Subscribe(_ =>
    //    //Console.WriteLine());

    //    //si.WhenValueChanged(t => t.SelectedItem)
    //    //    .Throttle(TimeSpan.FromMilliseconds(250))
    //    //    .Where(_ => _ != null)
    //    //     .Select(_ => _.Object).Subscribe(_ =>
    //    //Console.WriteLine());

    //    return si.WhenValueChanged(t => t.Output)
    //        .Throttle(TimeSpan.FromMilliseconds(250))
    //        .Where(_ => _ != null)
    //         .Select(_ => _.Object);

    //}

    //public static SelectableObject<T> AddSelectable<T>(this ISelectableItems<T> si, T s)
    //{
    //    var so = new SelectableObject<T>(s);
    //    so.WhenValueChanged(t => t.IsSelected)
    //        .Throttle(TimeSpan.FromMilliseconds(250))
    //       .Subscribe(b =>
    //       {
    //           si.SelectedItem = si.Items.FirstOrDefault(sof => sof.IsSelected == true);
    //       });
    //    return so;

    //}
}