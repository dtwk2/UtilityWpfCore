using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Interactive
{
    public class InteractiveCollectionFactory
    {
        private static bool Ft<T>(T o) => true;


        public static (InteractiveCollectionViewModel<T, object> collectionViewModel, IDisposable disposable) Build<T>(
        IObservable<T> elems,
        IObservable<Func<T, object>> getkeyObservable = null,
        IObservable<IFilter> filter = null,
        IObservable<T> deletedObservable = null,
        IObservable<Unit> clearedObervable = null,
        IObservable<bool> deleteableObervable = null,
           IObservable<bool> isCheckableObervable = null,
        //IObservable<bool> isReadOnly = null,
        IObservable<bool> doubleClickToCheck = null)
        {

            var (sx, exs, dis) = CreateChangeSet(elems, getkeyObservable, filter, deletedObservable, clearedObervable);

            var interactivecollection = new InteractiveCollectionViewModel<T, object>(
                sx,
                //visiblefilter: deletedObservable.Select(a => GetPredicate<T>()),
                enabledfilter: Observable.Return<Predicate<T>>(a => true),
                deletable: deleteableObervable,
                getkey: getkeyObservable,
                doubleClickToCheck: doubleClickToCheck,
                checkable:isCheckableObervable);

            var dis2 = exs.Subscribe(e=>interactivecollection.OnError(e));

            return (interactivecollection, new CompositeDisposable(dis,dis2));
        }




        public static (InteractiveCollectionBase<object> collectionBase, IDisposable disposable) BuildGroup<T>(
            IObservable<T> elems,
            IObservable<Func<T, object>> getkeyObservable,
       IObservable<IFilter> filter = null,
       IObservable<T> deletedObservable = null,
       IObservable<Unit> clearedObervable = null,
       IObservable<bool> deleteableObervable = null,
       //IObservable<bool> isReadOnly = null,
       IObservable<bool> doubleClickToCheck = null,
       IObservable<string> groupParameter = null
       )
        {
            var dx = Observable.Create<string>(_ => () => { });

            InteractiveCollectionGroupViewModel<T, object> interactivecollection = null;
            var (sx, exs, dis) = CreateChangeSet(elems, getkeyObservable, filter, deletedObservable, clearedObervable);

            interactivecollection = new InteractiveCollectionGroupViewModel<T, object>(
                sx,
                groupParameter,
                enabledfilter: Observable.Return<Predicate<T>>(a => true),
                deletable: deleteableObervable,
                getkey: getkeyObservable,
                doubleClickToCheck: doubleClickToCheck);

            var dis2 = exs.Subscribe(e => interactivecollection.OnError(e));

            return (interactivecollection, new CompositeDisposable(dis,dis2)) ;
        }




        public static (IObservable<IChangeSet<T, object>>, IObservable<Exception>, IDisposable) CreateChangeSet<T>(
 IObservable<T> elems,
 IObservable<Func<T, object>> getkeyObservable = null,
 IObservable<IFilter> filterObservable = null,
 IObservable<T> deletedObservable = null,
 IObservable<Unit> clearedObervable = null)
        {
            ISubject<Exception> exs = new Subject<Exception>();

            var sx = ObservableChangeSet.Create(cache =>
            {
                var composite = new CompositeDisposable();

                var dels = deletedObservable?/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/
                .Subscribe(a =>
                {
                    try
                    {
                        cache.Remove(a);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error removing " + a.ToString() + " from cache");
                        Console.WriteLine(ex.Message);
                        exs.OnNext(ex);
                        //ArgumentNullException
                    }
                })?.DisposeWith(composite);

                var clear = clearedObervable?
                .Subscribe(_ => cache.Clear())
                ?.DisposeWith(composite);

                var se  = elems
                .Subscribe(item =>
                {
                    try
                    {
                        cache.AddOrUpdate(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error adding " + item.ToString() + " from cache");
                        Console.WriteLine(ex);
                        exs.OnNext(ex);
                    }
                })?.DisposeWith(composite);

                return composite;
            }, new Func<T, object>(a => a));

            if (filterObservable != null)
                sx = sx.Filter(filterObservable.Select(filter =>
                {
                    bool f(T filtrate) => filter.Filter(filtrate);
                    return (Func<T, bool>)f;
                }).StartWith(Ft));

            var dis = getkeyObservable?.Subscribe(_ =>
            {
                sx.ChangeKey(_);
            });

            return (sx, exs, dis);
        }



        //public static (InteractiveCollectionViewModel<T, object> collectionViewModel, IDisposable disposable) Build<T>(
        // IObservable<IEnumerable<T>> elems,
        // IObservable<Func<T, object>> getkeyObservable = null,
        // IObservable<IFilter> filter = null,
        // IObservable<T> deletedObservable = null,
        // IObservable<Unit> clearedObervable = null,
        // IObservable<bool> deleteableObservable = null,
        // IObservable<bool> isCheckableObservable = null,
        // IObservable<bool> isReadOnly = null,
        // IObservable<bool> doubleClickToCheck = null)
        //{
        //    return Build(elems.SelectMany(a => a), getkeyObservable, filter, deletedObservable, clearedObervable, deleteableObservable, isCheckableObservable, isReadOnly, doubleClickToCheck);
        //}

        //public static (InteractiveCollectionBase<object> collectionBase, IDisposable disposable) BuildGroup<T>(
        //    IObservable<IEnumerable<T>> elems,
        //    IObservable<Func<T, object>> getkeyObservable,
        //    IObservable<IFilter> filter = null,
        //    IObservable<T> deletedObservable = null,
        //    IObservable<Unit> clearedObervable = null,
        //    IObservable<bool> deleteableObervable = null,
        //    IObservable<bool> isReadOnly = null,
        //    IObservable<bool> doubleClickToCheck = null,
        //    IObservable<string> groupParameter = null)
        //{
        //    return BuildGroup(elems.SelectMany(a => a), getkeyObservable, filter, deletedObservable, clearedObervable, deleteableObervable, isReadOnly, doubleClickToCheck, groupParameter);
        //}

        //void fds<T, R, S>()
        //{
        //    IObservable<IChangeSet<T, R>> changeSet = null;
        //    Func<T, S> func = null;

        //    var Collection = changeSet.Group(func);

        //}

        //static Predicate<T> GetPredicate<T>()
        //{
        //    return f;

        //    static bool f(T o) => true;
        //}



        //public static InteractiveCollectionViewModel<T, Guid> Build2<T>(
        //    IObservable<T> elems,
        //    IObservable<IFilter> filter = null,
        //    IObservable<T> deletedObservable = null,
        //    IObservable<Unit> clearedObservable = null)
        //{
        //    ISubject<Exception> exs = new Subject<Exception>();

        //    var composite = new CompositeDisposable();

        //    var sx = ObservableChangeSet.Create<T, Guid>(cache =>
        //     {
        //         deletedObservable?/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/.Subscribe(_ =>
        //         {
        //             try
        //             {
        //                 cache.Remove(_);
        //             }
        //             catch (Exception ex)
        //             {
        //                 Debug.WriteLine("error removing " + _.ToString() + " from cache");
        //                 Debug.WriteLine(ex.Message);
        //                 exs.OnNext(ex);
        //                 //ArgumentNullException
        //             }
        //         })
        //         ?.DisposeWith(composite);

        //         clearedObservable?.Subscribe(_ => cache.Clear())?.DisposeWith(composite);

        //         elems.Subscribe(elem =>
        //         {
        //             try
        //             {
        //                 cache.AddOrUpdate(elem);
        //             }
        //             catch (Exception ex)
        //             {
        //                 Debug.WriteLine("error adding " + elem.ToString() + " from cache");
        //                 Debug.WriteLine(ex);
        //                 exs.OnNext(ex);
        //             }
        //         })
        //        ?.DisposeWith(composite);

        //         return composite;

        //     }, a => Guid.NewGuid());


        //    sx = filter == null ? sx : sx

        //        .Filter(filter.Select(_ =>
        //    {
        //        bool f(T aa) => _.Filter(aa);
        //        return (Func<T, bool>)f;
        //    })
        //    .StartWith(Ft));

        //    var interactivecollection = new InteractiveCollectionViewModel<T, Guid>(sx);

        //    exs.Subscribe(ex =>
        //    (interactivecollection.Errors as ISubject<Exception>).OnNext(ex));

        //    return interactivecollection;
        //}

    }
}