﻿using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;
using UtilityWpf.Interactive.Abstract;
using UtilityWpf.Interactive.Model;
using UtilityWpf.Property;
using UtilityWpf.Model;

namespace UtilityWpf.Interactive
{
    public class InteractiveCollectionViewModel<T> : InteractiveCollectionBase<T>, ICollectionViewModel<IObject<T>>, IDisposable
    {
        private readonly IDisposable disposable;

        public InteractiveCollectionViewModel(
            IObservable<IChangeSet<T>> observable,
            IObservable<Predicate<T>> visiblefilter = null,
            IObservable<Predicate<T>> enabledfilter = null,
            IObservable<Func<T, IConvertible>> getkey = null,
            string title = null)
        {
            disposable = observable
            .Transform(s =>
            {
                var so = new InteractiveObject<T>(s, visible: visiblefilter?.Select(a => (bool?)a(s)), enabled: enabledfilter?.Select(a => (bool?)a(s)), id: getkey);
                this.ReactToChanges(so);
                return (IObject<T>)so;
            })
              .Bind(out items)
              .DisposeMany()
              .Subscribe(_ =>
   Console.WriteLine("generic view model changed"),
                          ex =>
                          {
                              (Errors as ISubject<Exception>).OnNext(ex);
                              Console.WriteLine("Error in generic view model");
                          });

            Title = title;
        }

        public InteractiveCollectionViewModel(IEnumerable<T> enumerable, string childrenpath, IObservable<bool> ischecked, IObservable<bool> expand, System.Windows.Threading.Dispatcher dispatcher = null, Func<T, IConvertible> getkey = null, string title = null)
        {
            var xs = enumerable.Select
             (s =>
             {
                 var so = new ChildInteractiveObject<T>(s, childrenpath, ischecked, expand, getkey?.Invoke(s));
                 return so;
             }).ToList();

            foreach (var so in xs)
            {
                this.ReactToChanges(so);
                so.OnPropertyChangeWithSource<ChildInteractiveObject<T>, KeyValuePair<T, InteractionArgs>>(nameof(ChildInteractiveObject<T>.ChildChanged)).Subscribe(_ =>
                {
                    childSubject.OnNext(_.Item2);
                });
            }

            items = new ReadOnlyObservableCollection<IObject<T>>(new ObservableCollection<IObject<T>>(xs));

            ischecked.DelaySubscription(TimeSpan.FromSeconds(0.5)).Take(1).Subscribe(_ =>
            {
                foreach (var x in xs)
                    x.IsChecked = _;
            });

            Title = title;
        }

        //   public InteractiveCollectionViewModel(IObservable<T> observable, Func<T, IConvertible> getkey = null, string title = null)
        //   {
        //       observable.ToObservableChangeSet()
        //           .Transform
        //           (s =>
        //           {
        //               var so = new SHDObject<T>(s, null, null, getkey?.Invoke(s));

        //               this.ReactToChanges(so);
        //               return (IObject<T>)so;
        //           })
        //.Bind(out items)
        //    .DisposeMany()
        //      .Subscribe(
        //      _ =>
        //      Console.WriteLine("generic view model changed"),
        //           ex =>
        //           {
        //               (Errors as ISubject<Exception>).OnNext(ex);
        //               Console.WriteLine("Error in generic view model");
        //           });

        //       Title = title;
        //   }

        //public InteractiveCollectionViewModel(IEnumerable<T> enumerable, Func<T, IConvertible> getkey = null, string title = null)
        //{
        //    var xx = enumerable.Select
        //     (s =>
        //     {
        //         var so = new SHDOObject(s, null, null, getkey?.Invoke(s));
        //         return (IObject<T>)so;
        //     }).ToList();

        //    foreach (var so in xx)
        //        this.ReactToChanges((SHDObject<T>)so);

        //    items = new ReadOnlyObservableCollection<IObject<T>>(new ObservableCollection<IObject<T>>(xx));

        //    Title = title;
        //}

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}