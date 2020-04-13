using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf.Common.Utility
{
    public static class VirtualisationHelper
    {
        public static IObservable<IChangeSet<T>> CreateChangeSet<T>(IEnumerable<T> items, IObservable<IVirtualRequest> virtualRequests, int initialSize)
        {
            return ObservableChangeSet.Create<T>(observableList =>
            {
                return SelectReplacementItems(virtualRequests, observableList, initialSize, items)
                              .Subscribe(a =>
                              {
                                  if (a.i < a.count)
                                      observableList.ReplaceAt(a.i, a.item);
                              });

                static IObservable<(int count, int i, T item)> SelectReplacementItems(IObservable<IVirtualRequest> virtualRequests, ISourceList<T> observableList, int initialSize, IEnumerable<T> items)
                {

                    return SelectMultipleThrottledItems(virtualRequests, observableList, initialSize, IndexAndCacheItems(items))
                          // Add default items to increase size of scrollviewer to that of InitialSize
                          .Do(a => observableList.AddRange(Enumerable.Range(0, initialSize).Select(_ => Activator.CreateInstance<T>())))
                          .Switch()
                          .ObserveOnDispatcher()
                          .Distinct(a => a.i);

                    // items needs to be indexed and cached
                    static IEnumerable<(int, T)> IndexAndCacheItems(IEnumerable<T> items) => new Endless.CachedEnumerable<(int, T)>(items.Select((a, i) => (i, a)));

                    static IObservable<IObservable<(int count, int i, T item)>> SelectMultipleThrottledItems(IObservable<IVirtualRequest> virtualRequests, ISourceList<T> observableList, int initialSize, IEnumerable<(int i, T pvm)> cached)
                    {
                        return virtualRequests
                                    // Select parameters of items that are visible 
                                    .Select(a => cached.Skip(a.StartIndex).Take(a.Size).LastOrDefault().i + a.Size)
                                    // Every time the requestd items last index + size of requested items exceeds size of obsevablelist create new observable
                                    .Where(a => a >= observableList.Count)
                                    // Delay to limit possibility of error when scroll thumb moved to bottom of scroll-bar
                                    .SampleFirst(TimeSpan.FromSeconds(3))
                                    .Select(sa => SelectThrottledItems(virtualRequests, cached, observableList));


                        // Throttle to delay display of items until scrolling has stopped.
                        static IObservable<(int count, int i, T item)> SelectThrottledItems(IObservable<IVirtualRequest> virtualRequests, IEnumerable<(int i, T pvm)> cached, ISourceList<T> observableList)
                        {
                            return virtualRequests
                                    .Throttle(TimeSpan.FromSeconds(0.5))
                                    .SelectMany(SelectItems);

                            IObservable<(int count, int i, T item)> SelectItems(IVirtualRequest a)
                            {
                                return cached.Skip(a.StartIndex).Take(a.Size).Select(v => (observableList.Count, v.i, v.pvm)).ToObservable();
                            }
                        }
                    }
                }
            });
        }
    }
}

//https://github.com/tompazourek/Endless/blob/master/src/Endless/CachedEnumerable/Internal/CachedEnumerable.cs
namespace Endless
{
    /// <summary>
    /// <see cref="ICachedEnumerable{T}" />
    /// </summary>
    internal class CachedEnumerable<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _cache = new LinkedList<T>();
        private readonly IEnumerator<T> _sourceEnumerator;
        private bool _disposed;

        internal CachedEnumerable(IEnumerable<T> source)
        {
            _sourceEnumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _sourceEnumerator.Dispose();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_disposed)
                throw new ObjectDisposedException("CachedEnumerable was already disposed");

            return new CachedEnumerator<T>(_cache.GetEnumerator(), _sourceEnumerator, this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class CachedEnumerator<TT> : IEnumerator<TT>
        {
            private readonly IEnumerator<TT> _first;
            private readonly CachedEnumerable<TT> _parent;
            private readonly IEnumerator<TT> _second;
            private bool _switched; // switched from first to second

            public CachedEnumerator(IEnumerator<TT> firstEnumerator, IEnumerator<TT> secondEnumerator, CachedEnumerable<TT> parent)
            {
                _first = firstEnumerator;
                _second = secondEnumerator;
                _parent = parent;
                Current = _first.Current;
            }

            public void Dispose()
            {
                _first.Dispose();
            }

            public bool MoveNext()
            {
                if (!_switched && _first.MoveNext())
                {
                    Current = _first.Current;
                    return true;
                }

                _switched = true;

                if (!_second.MoveNext())
                    return false;

                Current = _second.Current;
                _parent._cache.AddLast(Current); // add to cache

                return true;
            }

            public void Reset()
            {
                _first.Reset();
            }

            public TT Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }
}

