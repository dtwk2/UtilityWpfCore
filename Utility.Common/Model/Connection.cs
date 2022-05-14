using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common.Model
{
    public class Connection<TIn, TOut> : IObserver<TIn>, IObservable<TOut>, IDisposable
    {
        ReplaySubject<TIn> inSubject = new(1);
        ReplaySubject<TOut> outSubject = new(1);
        private IDisposable disposable;

        private Connection(Func<IObservable<TIn>, IObservable<TOut>> func)
        {
            disposable = func(inSubject).Subscribe(outSubject);
        }

        //protected IObserver<TIn> In => inSubject;
        //protected IObservable<TOut> Out => outSubject;


        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TIn value)
        {
            inSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return outSubject.Subscribe(observer);
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public static Connection<TIn, TOut> Create(Func<IObservable<TIn>, IObservable<TOut>> func)
        {
            return new Connection<TIn, TOut>(func);
        }
    }
}
