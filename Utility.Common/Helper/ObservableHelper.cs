using System;
using System.Linq;
using System.Reactive.Linq;

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
    }
}