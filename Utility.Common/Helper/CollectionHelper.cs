using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace Utility.Common.Helper
{
    public static class CollectionHelper
    {
        public static ReadOnlyObservableCollection<T> ToCollection<T, TKey>(this IObservable<IChangeSet<T, TKey>> observable, out IDisposable disposable)
            where TKey : notnull
        {
            disposable = observable
                .Bind(out var collection)
                .Subscribe();

            return collection;
        }

        public static ReadOnlyObservableCollection<T> ToCollection<T>(this IObservable<IChangeSet<T>> observable, out IDisposable disposable)
        {
            disposable = observable
                .Bind(out var collection)
                .Subscribe();

            return collection;
        }

        public static ReadOnlyObservableCollection<T> ToCollection<T, TKey>(this IObservable<IChangeSet<T, TKey>> observable, CompositeDisposable disposable)
            where TKey : notnull
        {
            _ = observable
                .Bind(out var collection)
                .Subscribe()
                .DisposeWith(disposable);

            return collection;
        }

        public static ReadOnlyObservableCollection<T> ToCollection<T>(this IObservable<IChangeSet<T>> observable, CompositeDisposable disposable)
        {
            _ = observable
                .Bind(out var collection)
                .Subscribe()
                .DisposeWith(disposable);

            return collection;
        }
    }
}