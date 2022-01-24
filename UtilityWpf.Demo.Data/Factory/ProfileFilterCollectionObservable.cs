using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Endless;
using LanguageExt;
using ReactiveUI;
using UtilityInterface.NonGeneric;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Data.Factory
{
    public sealed class ProfileFilterCollectionObservable : IObservable<ProfileFilter>
    {
        private readonly ProfileFilter[] profiles =
        {
            new PositiveProfileFilter(),
            new NegativeProfileFilter(),
            new RandomProfileFilter(),
            new TopFilter(5)
        };

        public IDisposable Subscribe(IObserver<ProfileFilter> observer)
        {
            return profiles
                    .ToObservable()
                    .Subscribe(observer);
        }
    }

    public class PositiveProfileFilter : ProfileFilter, IPredicate
    {
        public PositiveProfileFilter() : base("Positive")
        {
        }

        public override bool Invoke(object value)
        {
            return true;
        }
    }

    public class NegativeProfileFilter : ProfileFilter, IPredicate
    {
        public NegativeProfileFilter() : base("Negative")
        {
        }

        public override bool Invoke(object value)
        {
            return false;
        }
    }

    public class RandomProfileFilter : ProfileFilter, IPredicate
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


    public class TopFilter : ProfileFilter, IRefresh
    {
        private record ObjectFlag(object Value, int Index);
        private readonly List<object> masterObjects = new();
        private int count;
        private Dictionary<object, Enumerator<ObjectFlag>> indices;

        public TopFilter(int count) : base("Top")
        {
            Count = count;
            this.WhenAnyValue(a => a.Count)
                .Subscribe(a => Refresh());
        }

        public override bool Invoke(object value)
        {
            masterObjects.Add(value);

            if (indices == null)
                Refresh();

            if (indices.ContainsKey(value) == false)
            {
                indices[value] = new Enumerator<ObjectFlag>(new[] { Value() });
            }

            while (!indices[value].MoveNext())
            {
                indices[value].Add(Value());
                indices[value].Reset();
            }

            return (indices[value] as IEnumerator<ObjectFlag>).Current?.Index < count;

            ObjectFlag Value()
            {
                return new ObjectFlag(value, masterObjects.Count - 1);
            }
        }

        public int Count { get => count; set => this.RaiseAndSetIfChanged(ref count, value); }

        public void Refresh()
        {
            if (!masterObjects.Any()) return;

            indices = masterObjects
                .Select((a, i) => new ObjectFlag(a, i))
                .GroupBy(a => a.Value)
                .ToDictionary(grp => grp.Key, grp => new Enumerator<ObjectFlag>(grp));
            masterObjects.Clear();
        }
    }
}