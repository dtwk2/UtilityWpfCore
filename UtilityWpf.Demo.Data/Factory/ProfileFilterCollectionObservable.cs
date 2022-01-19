using System;
using System.Reactive.Linq;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Demo.Data.Model
{
    public sealed class ProfileFilterCollectionObservable : IObservable<ProfileFilter>
    {
        private readonly ProfileFilter[] profiles = new ProfileFilter[] { new PositiveProfileFilter(), new NegativeProfileFilter(), new RandomProfileFilter() };

        public ProfileFilterCollectionObservable()
        {

        }

        public IDisposable Subscribe(IObserver<ProfileFilter> observer)
        {
            return profiles
                    .ToObservable()
                    .Subscribe(observer);
        }
    }

    public abstract class ProfileFilter : IPredicate, IKey
    {
        public ProfileFilter(string header)
        {
            Header = header;
        }

        public string Header { get; }

        public abstract bool Check(object value);

        public string Key => Header;
    }

    public class PositiveProfileFilter : ProfileFilter, IPredicate
    {
        public PositiveProfileFilter() : base("Positive")
        {
        }

        public override bool Check(object value)
        {
            return true;
        }
    }

    public class NegativeProfileFilter : ProfileFilter, IPredicate
    {
        public NegativeProfileFilter() : base("Negative")
        {
        }

        public override bool Check(object value)
        {
            return false;
        }
    }

    public class RandomProfileFilter : ProfileFilter, IPredicate
    {
        Random random = new();
        public RandomProfileFilter() : base("Random")
        {
        }

        public override bool Check(object value)
        {
            return random.Next(0, 2) > 0;
        }
    }
}