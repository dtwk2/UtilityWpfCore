using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.ViewModel
{
    public class InteractiveCollectionViewModel : InteractiveCollectionBase,IDisposable
    {
        private readonly IDisposable disposable;

        public InteractiveCollectionViewModel(IObservable<IChangeSet<object>> observable, IObservable<object> visible = null, IObservable<object> disable = null, Func<object, IConvertible> getkey = null, string title = null, bool isReadonly = false)
        {
            disposable = observable

                .Transform(s =>
                {
                    var readableObservable = disable?.Scan(new List<object>(), (a, b) => { a.Add(b); return a; }).Select(fa);
                    var visibleObservable = visible?.Scan(new List<object>(), (a, b) => { a.Add(b); return a; }).Select(fa);
                    var so = new SHDOObject(s, visibleObservable, readableObservable, getkey?.Invoke(s))
                    {
                        IsReadOnly = isReadonly
                    };
                    this.ReactToChanges(so);
                    return (IObject)so;
                })
                 .Bind(out items)

                 .DisposeMany()
                 .Subscribe(
               _ =>
               {
                   foreach (var x in _.Select(a => new KeyValuePair<IObject, ListChangeReason>(a.Item.Current, a.Item.Reason)))
                       (Changes as Subject<KeyValuePair<IObject, ListChangeReason>>).OnNext(x);
               },
                ex =>
                {
                    (Errors as ISubject<Exception>).OnNext(ex);
                    Console.WriteLine("Error in generic view model");
                },
                () =>
                Console.WriteLine("observable completed"));

            Title = title;

            Predicate<object> fa(List<object> list)
            {
                return f;
                bool f(object a) => !list.Any(b => b.Equals(a));
            }
         
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
