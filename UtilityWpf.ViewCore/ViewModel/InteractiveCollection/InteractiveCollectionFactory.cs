using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.ViewModel
{
    public class InteractiveCollectionFactory
    {
        public static ViewModel.InteractiveCollectionViewModel<T, object> Build<T>(
            Func<T, object> getkey,
            IObservable<IEnumerable<T>> elems,
            IObservable<IFilter> filter,
            IObservable<T> DeletedSubject,
            IObservable<object> ClearedSubject,
            //IObservable<object> groupSubject =null,
            IObservable<Func<T, object>> getkeys = null, bool isReadOnly = false)
        {
            var dx = Observable.Create<string>(_ => () => { });

            ViewModel.InteractiveCollectionViewModel<T, object> interactivecollection = null;
            ISubject<Exception> exs = new Subject<Exception>();
            var sx = ObservableChangeSet.Create(cache =>
            {
                var dels = DeletedSubject/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/.Subscribe(_ =>
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

                ClearedSubject.Subscribe(_ => cache.Clear());

                elems.Subscribe(_ =>
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


                return new System.Reactive.Disposables.CompositeDisposable(dels);
            }, getkey)
            .Filter(filter.Select(_ =>
               {
                   Func<T, bool> f = aa => _.Filter(aa);
                   return f;
               })
      
               .StartWith(ft));

            //Func<T, object> func = (a) =>
            //  {

            //  };

            //sx.Group(func);

            getkeys?.Subscribe(_ =>
            {
                sx.ChangeKey(_);
            });

            interactivecollection = new ViewModel.InteractiveCollectionViewModel<T, object>(sx, DeletedSubject, getkey:_ => (IConvertible)getkey(_));

            exs.Subscribe(ex =>
            (interactivecollection.Errors as System.Reactive.Subjects.ISubject<Exception>).OnNext(ex));

            return interactivecollection;
        }

        public static IObservable<ViewModel.InteractiveCollectionViewModel<T, object>> Build<T>(IObservable<Func<T, object>> getkeys, IObservable<IEnumerable<T>> elems, IObservable<IFilter> filter, IObservable<T> DeletedSubject, IObservable<object> ClearedSubject, System.Reactive.Concurrency.DispatcherScheduler UI)
        {
            return getkeys?.Select(_ =>
            {
                return Build(_, elems, filter, DeletedSubject, ClearedSubject);
            });
        }

        private static bool ft<T>(T o) => true;



        public static ViewModel.InteractiveCollectionViewModel<T, Guid> Build<T>(IObservable<T> elems, IObservable<IFilter> filter, IObservable<T> DeletedSubject, IObservable<Unit> ClearedSubject = null)
        {
            ISubject<Exception> exs = new Subject<Exception>();
           
            var sx = ObservableChangeSet.Create<T,Guid>(cache =>
            {
                var dels = DeletedSubject/*.WithLatestFrom(RemoveSubject.StartWith(Remove).DistinctUntilChanged(), (d, r) => new { d, r })*/.Subscribe(_ =>
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
                });

                ClearedSubject?.Subscribe(_ => cache.Clear());

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
                });
                return new System.Reactive.Disposables.CompositeDisposable(dels);
            }, a => Guid.NewGuid())
            .Filter(filter.Select(_ =>
            {
                Func<T, bool> f = aa => _.Filter(aa);
                return f;
            })
            .StartWith(ft));

            ViewModel.InteractiveCollectionViewModel<T, Guid> interactivecollection = new ViewModel.InteractiveCollectionViewModel<T, Guid>(sx);

            exs.Subscribe(ex =>
            (interactivecollection.Errors as System.Reactive.Subjects.ISubject<Exception>).OnNext(ex));

            return interactivecollection;
        }

    }
}