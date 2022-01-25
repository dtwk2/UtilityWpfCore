using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Endless;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Data.Factory
{
    public sealed class ProfileCollectionObservable : IObservable<Profile>, IDisposable
    {
        private readonly int startCount;
        private readonly int speed;
        private IDisposable? observable;
        private readonly Lazy<ReadOnlyCollection<Profile>> pool = new(() => ProfileFactory.BuildPool());
        private ReplaySubject<Profile> profiles = new();

        public ProfileCollectionObservable(int startCount = 0, int speed = 1)
        {
            this.startCount = startCount;
            this.speed = speed;
        }

        public IDisposable Subscribe(IObserver<Profile> observer)
        {
            observable ??= Observable
              .Interval(TimeSpan.FromSeconds(speed))
              .StartWith(Enumerable.Repeat(0L, startCount).ToArray())
              .ObserveOnDispatcher()
              .Select(a => pool.Value.Random())
              .Subscribe(profiles);

            return profiles
                    .Subscribe(observer);
        }

        public void Dispose()
        {
            observable?.Dispose();
        }
    }
}