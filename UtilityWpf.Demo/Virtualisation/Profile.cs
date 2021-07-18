using BFF.DataVirtualizingCollection.DataVirtualizingCollection;
using DynamicData;
using Endless;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using UtilityWpf.TestData;

namespace UtilityWpf.DemoApp
{
    public class ProfileCollectionTimed
    {
        private const int _speed = 3;

        private readonly ReadOnlyObservableCollection<ProfileViewModel> profiles;

        public ReadOnlyObservableCollection<ProfileViewModel> Profiles => profiles;

        public ProfileCollectionTimed(int speed)
        {
            var pool = ProfileFactory.BuildPool();
            _ = Observable.Interval(TimeSpan.FromSeconds(speed))
                //.ObserveOnDispatcher()
                       .Select(a => pool.Random())
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        public ProfileCollectionTimed()
        {
            var pool = ProfileFactory.BuildPool();
            _ = Observable.Interval(TimeSpan.FromSeconds(_speed))
                //.ObserveOnDispatcher()
                       .Select(a => pool.Random())
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        private class comparer : IComparer<ProfileViewModel>
        {
            public int Compare([AllowNull] ProfileViewModel x, [AllowNull] ProfileViewModel y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
    }

    public class ProfileCollectionSlow : ProfileCollectionTimed
    {
        public ProfileCollectionSlow() : base(6)
        {
        }
    }

    public class ProfileCollectionVirtualise1 : ReactiveObject
    {
        private int val = 1;
        private readonly ObservableAsPropertyHelper<IList<ProfileViewModel>> profiles;

        public ProfileCollectionVirtualise1()
        {
            profiles = this.WhenAnyValue(a => a.Value).Select(a => GetProfiles(a)).ToProperty(this, a => a.Profiles);

            IList<ProfileViewModel> GetProfiles(int i)
            {
                var ProfilePool = ProfileFactory.BuildPool();
                return DataVirtualizingCollectionBuilder
                    .Build<ProfileViewModel>(i, RxApp.MainThreadScheduler)
                 .NonPreloading()
                 .Hoarding()
                 .NonTaskBasedFetchers(
                     (offset, pageSize) =>
                     {
                         Console.WriteLine($"{nameof(Profiles)}: Loading page with offset {offset}");
                         var range = Enumerable.Range(offset, pageSize).Select(i => ProfilePool[i % ProfilePool.Count]).ToArray();
                         return range;
                     },
                     () =>
                     {
                         Console.WriteLine($"{nameof(Profiles)}: Loading count");
                         return 420420;
                     })
                 .AsyncIndexAccess((_, __) => new ProfileViewModel());
            }
        }

        public IList<ProfileViewModel> Profiles => profiles.Value;

        public int Value { get => val; set => this.RaiseAndSetIfChanged(ref val, value); }
    }

    public class ProfileCollectionVirtualiseLimited
    {
        private readonly ReadOnlyObservableCollection<ProfileViewModel> profiles;

        /// <summary>
        /// Only adds to the pool of data when asked to
        /// </summary>
        /// <param name="virtualRequests"></param>
        public ProfileCollectionVirtualiseLimited(IObservable<IVirtualRequest> virtualRequests)
        {
            var pool = ProfileFactory.BuildPool();

            var cached = 0.Iterate(a => a + 1)
                 .Select(i => (i, pool.Random()))
                 .Cached();

            _ =
              virtualRequests
                .SelectMany(a => cached.Skip(a.StartIndex).Take(a.Size + 30))
               .ToObservableChangeSet(a => a.i)
               .Transform(a => a.Item2)
                        .Bind(out profiles)
                        .Subscribe();
        }

        public ReadOnlyObservableCollection<ProfileViewModel> Profiles => profiles;
    }

    public class ProfileCollectionVirtualise
    {
        private readonly ReadOnlyObservableCollection<ProfileViewModel> profiles;

        /// <summary>
        /// Creates an initial set of blank data then fills when requested
        /// </summary>
        /// <param name="virtualRequests"></param>
        /// <param name="initialSize"></param>
        public ProfileCollectionVirtualise(IObservable<IVirtualRequest> virtualRequests, int initialSize)
        {
            var pool = ProfileFactory.BuildPool();
            var items = new Func<ProfileViewModel>(pool.Random).Repeat();

            _ = VirtualisationHelper.CreateChangeSet(items, virtualRequests, initialSize)
                .Bind(out profiles)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<ProfileViewModel> Profiles => profiles;
    }
}