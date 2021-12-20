using DynamicData;
using Endless;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;

namespace UtilityWpf.Demo.Data.Model
{
    public class ProfileCollectionTimed
    {
        private const int _speed = 3;

        private readonly ReadOnlyObservableCollection<Profile> profiles;

        public ReadOnlyObservableCollection<Profile> Profiles => profiles;

        public ProfileCollectionTimed(int speed)
        {
            var pool = ProfileFactory.BuildPool();
            _ = Observable.Interval(TimeSpan.FromSeconds(speed))
               .StartWith(Enumerable.Repeat(0L, 30).ToArray())
                .ObserveOnDispatcher()
                       .Select(a => pool.Random())
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        public ProfileCollectionTimed()
        {
            var pool = ProfileFactory.BuildPool();
            _ = Observable.Interval(TimeSpan.FromSeconds(_speed))
                .ObserveOnDispatcher()
                       .Select(a => pool.Random())
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        private class comparer : IComparer<Profile>
        {
            public int Compare([AllowNull] Profile x, [AllowNull] Profile y)
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
}