using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility.Common.Helper
{
    public static class ObservableHelper
    {
        public static IObservable<(T, R)> Combine<T, R>(this IObservable<T> observable, R observed)
        {
            return observable
                .Scan((default(T), observed), (a, b) => (b, a.Item2))
                .Skip(1);
        }

        public static IObservable<(T?, TR?)> Combine<T, TR>(this IObservable<T> observable, IObservable<TR> observable2) 
            where T : struct
            where TR : struct
        {
            var subject = new ReplaySubject<(T?, TR?)>(1);

            observable.Subscribe(a =>
            {
                subject.OnNext((a, null));
            });

            observable2.Subscribe(b =>
            {
                subject.OnNext((null, b));
            });

            return subject;
        }

        public static IObservable<(T?, TR?)> CombineRefs<T, TR>(this IObservable<T> observable, IObservable<TR> observable2)
            where T : class
            where TR : class
        {
            var subject = new ReplaySubject<(T?, TR?)>(1);

            observable.Subscribe(a =>
            {
                subject.OnNext((a, null));
            });

            observable2.Subscribe(b =>
            {
                subject.OnNext((null, b));
            });

            return subject;
        }
    }
}