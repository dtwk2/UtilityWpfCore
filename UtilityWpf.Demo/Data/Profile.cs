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
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

    /// <summary>
    /// https://github.com/Yeah69/BFF.DataVirtualizingCollection
    /// </summary>
    ///
    internal class ProfileFactory
    {
        static ProfileFactory()
        {
        }

        private static BitmapImage GetImage(string path) => new BitmapImage(new Uri(System.IO.Path.GetFullPath("..\\..\\..\\" + path)));

        public static IReadOnlyList<ProfileViewModel> BuildPool() =>
                new ReadOnlyCollection<ProfileViewModel>(
                    new List<ProfileViewModel>
                    {
                    new ProfileViewModel(
                        "UI/UX designer",
                        "$55/hr",
                        "Wide Walson",
                        "Wade is a 32 year old UI/UX designer, with an impressive portfolio behind him.",
                        true,
                        false,
                        "Epic Coders",
                        new List<string>{ "UI", "UX", "photoshop" },
                        4,
                        GetImage("ProfilePics\\00_Wide.png")),
                    new ProfileViewModel(
                        "mobile designer",
                        "$32/hr",
                        "Paria Metrescu",
                        "Paria is an android and iOS developer who worked at Apple for 6 years.",
                        false,
                        true,
                        null,
                        new List<string>{ "PHP", "android", "iOS"},
                        2,
                        GetImage("ProfilePics/01_Paria.png")),
                    new ProfileViewModel(
                        "mobile designer",
                        "$42/hr",
                        "Morexandra Algan",
                        "Morexandra is a dedicated developer for mobile platforms and is very good at it.",
                        false,
                        true,
                        null,
                        new List<string>{ "PHP", "android", "iOS"},
                        12,
                         GetImage("ProfilePics/02_Morexandra.png")),
                    new ProfileViewModel(
                        "interactive designer",
                        "$44/hr",
                        "Smennifer Jith",
                        "Smennifer is an interactive designer who is really awesome at what she does.",
                        false,
                        true,
                        null,
                        new List<string>{ "PHP", "android", "iOS"},
                        2,
                         GetImage("ProfilePics/03_Smennifer.png")),
                    new ProfileViewModel(
                        "mobile designer",
                        "$40/hr",
                        "Anyetlana Svukova",
                        "Anyetlana is an Android and iOS designer with advanced knowledge in coding.",
                        true,
                        true,
                        null,
                        new List<string>{ "PHP", "android", "iOS"},
                        2,
                         GetImage("ProfilePics/04_Anyetlana.png")),
                    new ProfileViewModel(
                        "UI/UX designer",
                        "$30/hr",
                        "Korko van Maoh",
                        "Korko is a 25 year old web designer with an impressive portfolio behind him.",
                        false,
                        false,
                        "Visual Madness",
                        new List<string>{ "UI", "UX", "photoshop"},
                        4,
                         GetImage("ProfilePics/05_Korko.png")),
                    new ProfileViewModel(
                        "UX designer",
                        "$50/hr",
                        "Kowel Paszentka",
                        "Kowel is a 32 year old UX designer, with over 10 years of experience in what he does.",
                        false,
                        false,
                        "Apple Inc",
                        new List<string>{ "UI", "UX", "photoshop" },
                        4,
                         GetImage("ProfilePics/06_Kowel.png")),
                    new ProfileViewModel(
                        "mobile designer",
                        "$32/hr",
                        "Sinia Samionov",
                        "Sinia is an android and iOS developer who worked at Apple for 6 years.",
                        false,
                        true,
                        null,
                        new List<string>{ "PHP", "android", "iOS" },
                        2,
                         GetImage("ProfilePics/07_Sinia.png")),
                    new ProfileViewModel(
                        "photographer",
                        "$40/hr",
                        "Wonathan Jayne",
                        "Wonathan is a 28 year old photographer from London with real talent for what he does.",
                        false,
                        false,
                        "Epic Coders",
                        new List<string>{ "UI", "UX", "photoshop" },
                        4,
                         GetImage("ProfilePics/08_Wonathan.png")),
                    new ProfileViewModel(
                        "Superhero",
                        "free",
                        "Matban",
                        "I'm Matban!",
                        false,
                        true,
                        null,
                        new List<string>{ "tech", "IT", "martial arts" },
                        69,
                         GetImage("ProfilePics/09_Matban.png")),
                    new ProfileViewModel(
                        "mobile designer",
                        "$39/hr",
                        "Surgiana Geoclea",
                        "Surgiana is an android and iOS developer who worked at Apple for 6 years.",
                        false,
                        true,
                        null,
                        new List<string>{ "PHP", "android", "iOS" },
                        2,
                         GetImage("ProfilePics/10_Surgiana.png")),
                    new ProfileViewModel(
                        "UI/UX designer",
                        "$45/hr",
                        "Jogory Grehnes",
                        "Jogory is a 32 year old UI/UX designer, with an impressive portfolio behind him.",
                        false,
                        false,
                        "Epic Coders",
                        new List<string>{ "UI", "UX", "photoshop" },
                        4,
                         GetImage("ProfilePics/11_Jogory.png"))
                    });
    }

    public struct ProfileViewModel //: IEquatable<ProfileViewModel>
    {
        public static IValueConverter ToCompanyBrush =
            LambdaConverters.ValueConverter.Create<ProfileViewModel, Brush>(
                e => e.Value.IsFreelancer ? Brushes.Green : Brushes.Blue);

        public static IValueConverter ToCompanyText =
            LambdaConverters.ValueConverter.Create<ProfileViewModel, string>(
                e => e.Value.IsFreelancer ? "Freelancer" : e.Value.CompanyName);

        public static IValueConverter ToCompanyIcon =
            LambdaConverters.ValueConverter.Create<ProfileViewModel, MaterialDesignThemes.Wpf.PackIconKind>(
                e => e.Value.IsFreelancer ? MaterialDesignThemes.Wpf.PackIconKind.AccountAddOutline : MaterialDesignThemes.Wpf.PackIconKind.City);

        public static IValueConverter PrefixedHiddenAbilitiesCount =
            LambdaConverters.ValueConverter.Create<int, string>(
                e => $"+{e.Value}");

        public static IValueConverter ProfilesTitle =
            LambdaConverters.ValueConverter.Create<int, string>(
                e => $"Profiles ({e.Value})");

        public ProfileViewModel(
            string occupation,
            string salary,
            string name,
            string description,
            bool isAvailable,
            bool isFreelancer,
            string companyName,
            IReadOnlyList<string> abilities,
            int hiddenAbilitiesCount,
            ImageSource picture
           )
        {
            Occupation = occupation;
            Salary = salary;
            Name = name;
            Description = description;
            IsAvailable = isAvailable;
            IsFreelancer = isFreelancer;
            CompanyName = companyName;
            Abilities = abilities;
            HiddenAbilitiesCount = hiddenAbilitiesCount;
            Picture = picture;
        }

        //public int Index { get; set; }

        public string Occupation { get; }

        public string Salary { get; }

        public string Name { get; }

        public string Description { get; }

        public bool IsAvailable { get; }

        public bool IsFreelancer { get; }

        public string CompanyName { get; }

        public IReadOnlyList<string> Abilities { get; }

        public int HiddenAbilitiesCount { get; }

        public ImageSource Picture { get; }

        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as ProfileViewModel);
        //}

        //public bool Equals([AllowNull] ProfileViewModel other)
        //{
        //    return other != null &&
        //        Index == other.Index &&
        //           Occupation == other.Occupation &&
        //           Salary == other.Salary &&
        //           Name == other.Name &&
        //           Description == other.Description &&
        //           IsAvailable == other.IsAvailable &&
        //           IsFreelancer == other.IsFreelancer &&
        //           CompanyName == other.CompanyName &&
        //           EqualityComparer<IReadOnlyList<string>>.Default.Equals(Abilities, other.Abilities) &&
        //           HiddenAbilitiesCount == other.HiddenAbilitiesCount &&
        //           EqualityComparer<ImageSource>.Default.Equals(Picture, other.Picture);
        //}

        //public override int GetHashCode()
        //{
        //    var hash = new HashCode();
        //    hash.Add(Index);
        //    hash.Add(Occupation);
        //    hash.Add(Salary);
        //    hash.Add(Name);
        //    hash.Add(Description);
        //    hash.Add(IsAvailable);
        //    hash.Add(IsFreelancer);
        //    hash.Add(CompanyName);
        //    hash.Add(Abilities);
        //    hash.Add(HiddenAbilitiesCount);
        //    hash.Add(Picture);
        //    return hash.ToHashCode();
        //}

        //public static bool operator ==(ProfileViewModel left, ProfileViewModel right)
        //{
        //    return EqualityComparer<ProfileViewModel>.Default.Equals(left, right);
        //}

        //public static bool operator !=(ProfileViewModel left, ProfileViewModel right)
        //{
        //    return !(left == right);
        //}

        //public ProfileViewModel WithIndex(int index)
        //{
        //    return new ProfileViewModel(
        //    Occupation,
        //    Salary,
        //    Name,
        //    Description,
        //    IsAvailable,
        //    IsFreelancer,
        //    CompanyName,
        //    null,
        //    //Abilities,
        //    HiddenAbilitiesCount,
        //    null, // Picture,
        //    index);
        //}
    }
}