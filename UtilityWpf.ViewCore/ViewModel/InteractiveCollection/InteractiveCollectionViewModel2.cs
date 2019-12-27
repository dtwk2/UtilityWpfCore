using DynamicData;
using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;

namespace UtilityWpf.ViewModel
{
    public class InteractiveCollectionViewModel<T> : InteractiveCollectionBase<T>, ICollectionViewModel<IContain<T>>
    {
        public InteractiveCollectionViewModel(IObservable<IChangeSet<T>> observable,
IObservable<Predicate<T>> invisiblefilter,
IObservable<Predicate<T>> enabledfilter,
IScheduler scheduler, Func<T, IConvertible> getkey = null, string title = null)
        {
            observable.ObserveOn(scheduler)
           .Transform(s =>
           {
               var so = new SHDObject<T>(s, invisiblefilter, enabledfilter, getkey?.Invoke(s));
               this.ReactToChanges(so);
               return (IContain<T>)so;
           })
             .Bind(out items)

             .DisposeMany()
             .Subscribe(
           _ =>
           Console.WriteLine("generic view model changed"),
                         ex =>
                         {
                             (Errors as ISubject<Exception>).OnNext(ex);
                             Console.WriteLine("Error in generic view model");
                         });

            Title = title;
        }

        public InteractiveCollectionViewModel(IObservable<T> observable, IScheduler scheduler, Func<T, IConvertible> getkey = null, string title = null)
        {
            observable.ToObservableChangeSet().ObserveOn(scheduler)
                .Transform
                (s =>
                {
                    var so = new SHDObject<T>(s, null, null, getkey?.Invoke(s));

                    this.ReactToChanges(so);
                    return (IContain<T>)so;
                })
     .Bind(out items)
         .DisposeMany()
           .Subscribe(
           _ =>
           Console.WriteLine("generic view model changed"),
                ex =>
                {
                    (Errors as ISubject<Exception>).OnNext(ex);
                    Console.WriteLine("Error in generic view model");
                });

            Title = title;
        }

        public InteractiveCollectionViewModel(IEnumerable<T> enumerable, Func<T, IConvertible> getkey = null, string title = null)
        {
            var xx = enumerable.Select
             (s =>
             {
                 var so = new SHDOObject(s, null, null, getkey?.Invoke(s));
                 return (IContain<T>)so;
             }).ToList();

            foreach (var so in xx)
                this.ReactToChanges((SHDObject<T>)so);

            items = new ReadOnlyObservableCollection<IContain<T>>(new ObservableCollection<IContain<T>>(xx));

            Title = title;
        }

        public InteractiveCollectionViewModel(IEnumerable<T> enumerable, string childrenpath, IObservable<bool> ischecked, IObservable<bool> expand, System.Windows.Threading.Dispatcher dispatcher = null, Func<T, IConvertible> getkey = null, string title = null)
        {
            var xx = enumerable.Select
             (s =>
             {
                 var so = new SEObject<T>(s, childrenpath, ischecked, expand, dispatcher, getkey?.Invoke(s));

                 return (IContain<T>)so;
             }).ToList();

            foreach (var so in xx)
            {
                this.ReactToChanges((SEObject<T>)so);
                (so as SEObject<T>).OnPropertyChangeWithSource<SEObject<T>, KeyValuePair<T, InteractionArgs>>(nameof(SEObject<T>.ChildChanged)).Subscribe(_ =>
                {
                    ChildSubject.OnNext(_.Item2);
                });
            }

            items = new ReadOnlyObservableCollection<IContain<T>>(new ObservableCollection<IContain<T>>(xx));

            ischecked.DelaySubscription(TimeSpan.FromSeconds(0.5)).Take(1).Subscribe(_ =>
            {
                foreach (var x in xx)
                    (x as SEObject<T>).IsChecked = _;
            });

            Title = title;
        }
    }
}