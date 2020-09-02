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

namespace UtilityWpf.ViewModel
{
    public class InteractiveCollectionFactory
    {
        private static bool Ft<T>(T o) => true;

        public static InteractiveCollectionViewModel<T, object> Build<T>(
         IObservable<Func<T, object>> getkeyObservable,
            IObservable<IEnumerable<T>> elems,
            IObservable<IFilter> filter = null,
            IObservable<T> deletedObservable = null,
            IObservable<Unit> clearedObervable = null,
            IObservable<bool> deleteableObervable = null,
            IObservable<bool> isReadOnly = null,
            IObservable<bool> doubleClickToCheck = null,
            IObservable<string> groupParameter = null
            )
        {
            var dx = Observable.Create<string>(_ => () => { });

            InteractiveCollectionViewModel<T, object> interactivecollection = null;
            ISubject<Exception> exs = new Subject<Exception>();

            //var xx = getkeyObservable?.SelectMany(getKey =>
            //{
            //    var sx = ObservableChangeSet.Create(cache =>
            //    {
            //        var dels = deletedObservable/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/.Subscribe(_ =>
            //        {
            //            try
            //            {
            //                cache.Remove(_);
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine("error removing " + _.ToString() + " from cache");
            //                Console.WriteLine(ex.Message);
            //                exs.OnNext(ex);
            //                //ArgumentNullException
            //            }
            //        });

            //        clearedObervable?.Subscribe(_ => cache.Clear());

            //        elems.Subscribe(_ =>
            //        {
            //            foreach (var g in _)
            //                try
            //                {
            //                    cache.AddOrUpdate(g);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine("error adding " + g.ToString() + " from cache");
            //                    Console.WriteLine(ex);
            //                    exs.OnNext(ex);
            //                }
            //        });


            //        return new System.Reactive.Disposables.CompositeDisposable(dels);
            //    }, getKey);


            //    return sx;
            //})
            //.Filter(filter.Select(a =>
            //   {
            //       bool f(T aa) => a.Filter(aa);
            //       return (Func<T, bool>)f;
            //   })
            //   .StartWith(ft));


            var sx = ObservableChangeSet.Create(cache =>
            {
                var dels = deletedObservable/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/
                .Subscribe(_ =>
                {
                    try
                    {
                        cache.Remove(_);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error removing " + _.ToString() + " from cache");
                        Console.WriteLine(ex.Message);
                        exs.OnNext(ex);
                        //ArgumentNullException
                    }
                });

                clearedObervable?
                .Subscribe(_ => cache.Clear());

                elems
                .Subscribe(_ =>
                {
                    foreach (var g in _)
                        try
                        {
                            cache.AddOrUpdate(g);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("error adding " + g.ToString() + " from cache");
                            Console.WriteLine(ex);
                            exs.OnNext(ex);
                        }
                });

                return new CompositeDisposable(dels);
            }, new Func<T, object>(a => a))
                .Filter(filter.Select(a =>
                        {
                            bool f(T aa) => a.Filter(aa);
                            return (Func<T, bool>)f;
                        })
     
               .StartWith(Ft));

            //var merge = xx.Merge(sx);

            getkeyObservable?.Subscribe(_ =>
            {
                sx.ChangeKey(_);
            });

            interactivecollection = new InteractiveCollectionViewModel<T, object>(
                sx,
                //visiblefilter: deletedObservable.Select(a => GetPredicate<T>()),
                enabledfilter: Observable.Return<Predicate<T>>(a => true),
                deletable: deleteableObervable,
                getkey: getkeyObservable,
                doubleClickToCheck: doubleClickToCheck);

            exs.Subscribe(ex =>
            (interactivecollection.Errors as ISubject<Exception>).OnNext(ex));

            return interactivecollection;
        }


        public static InteractiveCollectionBase<object> BuildGroup<T>(
    IObservable<Func<T, object>> getkeyObservable,
       IObservable<IEnumerable<T>> elems,
       IObservable<IFilter> filter = null,
       IObservable<T> deletedObservable = null,
       IObservable<Unit> clearedObervable = null,
       IObservable<bool> deleteableObervable = null,
       IObservable<bool> isReadOnly = null,
       IObservable<bool> doubleClickToCheck = null,
       IObservable<string?> groupParameter = null
       )
        {
            var dx = Observable.Create<string>(_ => () => { });

            InteractiveCollectionGroupViewModel<T, object> interactivecollection = null;
            ISubject<Exception> exs = new Subject<Exception>();

            var sx = ObservableChangeSet.Create(cache =>
            {
                var dels = deletedObservable/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/
                .Subscribe(_ =>
                {
                    try
                    {
                        cache.Remove(_);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error removing " + _.ToString() + " from cache");
                        Console.WriteLine(ex.Message);
                        exs.OnNext(ex);
                        //ArgumentNullException
                    }
                });

                clearedObervable?
                .Subscribe(_ => cache.Clear());

                elems
                .Subscribe(_ =>
                {
                    foreach (var g in _)
                        try
                        {
                            cache.AddOrUpdate(g);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("error adding " + g.ToString() + " from cache");
                            Console.WriteLine(ex);
                            exs.OnNext(ex);
                        }
                });

                return new CompositeDisposable(dels);
            }, new Func<T, object>(a => a))
                .Filter(filter.Select(a =>
                {
                    bool f(T aa) => a.Filter(aa);
                    return (Func<T, bool>)f;
                })

               .StartWith(Ft));

            //var merge = xx.Merge(sx);

            getkeyObservable?.Subscribe(_ =>
            {
                sx.ChangeKey(_);
            });

            interactivecollection = new InteractiveCollectionGroupViewModel<T, object>(
                sx,
                groupParameter,
                enabledfilter: Observable.Return<Predicate<T>>(a => true),
                deletable: deleteableObervable,
                getkey: getkeyObservable,
                doubleClickToCheck: doubleClickToCheck);

            exs.Subscribe(ex =>
            (interactivecollection.Errors as ISubject<Exception>).OnNext(ex));

            return interactivecollection;
        }

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

        public static IObservable<InteractiveCollectionViewModel<T, object>> Build<T>(IObservable<Func<T, object>> getkeys, IObservable<IEnumerable<T>> elems, IObservable<IFilter> filter, IObservable<T> DeletedSubject, IObservable<Unit> ClearedSubject, System.Reactive.Concurrency.DefaultScheduler UI)
        {
            return getkeys?.Select(a =>
            {
                return Build(Observable.Return(a), elems, filter, DeletedSubject, ClearedSubject);
            });
        }

     

        public static InteractiveCollectionViewModel<T, Guid> Build<T>(
            IObservable<T> elems,
            IObservable<IFilter> filter = null,
            IObservable<T> deletedObservable = null,
            IObservable<Unit> clearedObservable = null)
        {
            ISubject<Exception> exs = new Subject<Exception>();

            var composite = new CompositeDisposable();

            var sx = ObservableChangeSet.Create<T, Guid>(cache =>
             {
                 deletedObservable?/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/.Subscribe(_ =>
                 {
                     try
                     {
                         cache.Remove(_);
                     }
                     catch (Exception ex)
                     {
                         Debug.WriteLine("error removing " + _.ToString() + " from cache");
                         Debug.WriteLine(ex.Message);
                         exs.OnNext(ex);
                         //ArgumentNullException
                     }
                 })
                 ?.DisposeWith(composite);

                 clearedObservable?.Subscribe(_ => cache.Clear())?.DisposeWith(composite);

                 elems.Subscribe(elem =>
                 {
                     try
                     {
                         cache.AddOrUpdate(elem);
                     }
                     catch (Exception ex)
                     {
                         Debug.WriteLine("error adding " + elem.ToString() + " from cache");
                         Debug.WriteLine(ex);
                         exs.OnNext(ex);
                     }
                 })
                ?.DisposeWith(composite);

                 return composite;

             }, a => Guid.NewGuid());


            sx = filter == null ? sx : sx

                .Filter(filter.Select(_ =>
            {
                bool f(T aa) => _.Filter(aa);
                return (Func<T, bool>)f;
            })
            .StartWith(Ft));

            var interactivecollection = new InteractiveCollectionViewModel<T, Guid>(sx);

            exs.Subscribe(ex =>
            (interactivecollection.Errors as ISubject<Exception>).OnNext(ex));

            return interactivecollection;
        }

    }
}