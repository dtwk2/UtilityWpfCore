using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.NonGeneric;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Data.Factory
{
    public sealed class ProfileFilterCollectionObservable : IObservable<Filter>
    {
        private readonly Filter[] profiles =
        {
            new PositiveProfileFilter(),
            new NegativeProfileFilter(),
            new RandomProfileFilter(),
            new TopFilter(5)
        };

        public IDisposable Subscribe(IObserver<Filter> observer)
        {
            return profiles
                    .ToObservable()
                    .Subscribe(observer);
        }
    }

    public class PositiveProfileFilter : Filter, IPredicate
    {
        public PositiveProfileFilter() : base("Positive")
        {
        }

        public override bool Invoke(object value)
        {
            return true;
        }
    }

    public class NegativeProfileFilter : Filter, IPredicate
    {
        public NegativeProfileFilter() : base("Negative")
        {
        }

        public override bool Invoke(object value)
        {
            return false;
        }
    }

    public class RandomProfileFilter : Filter, IPredicate
    {
        private readonly Random random = new();

        public RandomProfileFilter() : base("Random")
        {
        }

        public override bool Invoke(object value)
        {
            return random.Next(0, 2) > 0;
        }
    }

    public class TopFilter : ObserverFilter<Profile>
    {
        private record ObjectFlag(object Value, int Index);
        private int takeFromTopLimit;
        private readonly Subject<IChangeSet<Profile>> subjects = new();
        private readonly ReadOnlyObservableCollection<Profile> collection;

        public TopFilter(int count) : base("Top")
        {
            takeFromTopLimit = count;

            subjects
                .Bind(out collection)
                .Subscribe();
        }

        public override bool Invoke(object value)
        {
            return collection.IndexOf(value) < TakeFromTopLimit;
        }

        public int TakeFromTopLimit { get => takeFromTopLimit; set => this.RaiseAndSetIfChanged(ref takeFromTopLimit, value); }

        public override void OnNext(IChangeSet<Profile> value)
        {
            subjects.OnNext(value);
        }
    }
}