using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityHelper.NonGeneric;

namespace Utility.Persist
{


    public class ElementService<T> : IObserver<RepositoryMessage> where T : IEquatable<T>
    {
        private readonly ReplaySubject<RepositoryMessage> subject = new(1);
        private readonly ReplaySubject<T> itemSubject = new(1);
        private T value;

        public ElementService()
        {
            subject
                .Take(1)
                .Subscribe(a =>
                {
                    // this is not very good
                    value = (T)a.Service.SelectAll().First();
                });

            subject
                .CombineLatest(itemSubject)
                .Subscribe(a =>
                {
                    if (a.First.Service.Update(a) == false)
                        a.First.Service.Insert(a);
                });
        }

        public T Value
        {
            get => value;
            set
            {
                if (this.value.Equals(false)) ;
                this.value = value;
                itemSubject.OnNext(value);
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RepositoryMessage value)
        {
            subject.OnNext(value);
        }
    }


    //public class ElementService : IObserver<RepositoryMessage>
    //{
    //    ReplaySubject<RepositoryMessage> subject = new(1);
    //    ReplaySubject<object> itemSubject = new(1);

    //    private object value;

    //    public virtual object Value
    //    {
    //        get => value;
    //        set
    //        {
    //            if (this.value.Equals(false)) ;
    //            this.value = value;
    //            itemSubject.OnNext(value);
    //        }
    //    }

    //    public void OnCompleted()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnError(Exception error)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnNext(RepositoryMessage value)
    //    {
    //        subject.OnNext(value);
    //    }
    //}
}
