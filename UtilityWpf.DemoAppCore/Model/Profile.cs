using BFF.DataVirtualizingCollection;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UtilityWpf.DemoAppCore
{

    public class ProfileCollection : ReactiveObject
    {
        IList<ProfileViewModel> GetProfiles(int i)
        {
            var ProfilePool = ProfileFactory.BuildPool();
            return DataVirtualizingCollectionBuilder<ProfileViewModel>
             .Build(i)
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
             .SyncIndexAccess();
        }

        public IList<ProfileViewModel> Profiles => profiles.Value;

        int val = 1;
        private readonly ObservableAsPropertyHelper<IList<ProfileViewModel>> profiles;

        public int Value { get => val; set => this.RaiseAndSetIfChanged(ref val, value); }


        public ProfileCollection()
        {
            profiles = this.WhenAnyValue(a => a.Value).Select(a => GetProfiles(a)).ToProperty(this, a => a.Profiles);
        }
    }

    public class ProfileCollection2
    {
        private readonly ReadOnlyObservableCollection<ProfileViewModel> profiles;

        public ReadOnlyObservableCollection<ProfileViewModel> Profiles => profiles;

        public ProfileCollection2()
        { var pool = ProfileFactory.BuildPool();
            _ = Observable.Interval(TimeSpan.FromSeconds(3))
                .ObserveOnDispatcher()
                       .SelectMany(a => pool.OrderBy(a => Guid.NewGuid()).Take(1))
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        class comparer : IComparer<ProfileViewModel>
        {
            public int Compare([AllowNull] ProfileViewModel x, [AllowNull] ProfileViewModel y)
            {
               return x.Name.CompareTo(y.Name);
            }
        }
    }


    public class ProfileCollectionSlow
    {
        private readonly ReadOnlyObservableCollection<ProfileViewModel> profiles;

        public ReadOnlyObservableCollection<ProfileViewModel> Profiles => profiles;

        public ProfileCollectionSlow()
        {
            var pool = ProfileFactory.BuildPool();
            _ = Observable.Interval(TimeSpan.FromSeconds(6))
                .ObserveOnDispatcher()
                 .SelectMany(a => pool.OrderBy(a=>Guid.NewGuid()).Take(1))
                 .StartWith(pool)
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        class comparer : IComparer<ProfileViewModel>
        {
            public int Compare([AllowNull] ProfileViewModel x, [AllowNull] ProfileViewModel y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
    }

    /// <summary>
    /// https://github.com/Yeah69/BFF.DataVirtualizingCollection
    /// </summary>
    /// 
    class ProfileFactory
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



    public class ProfileViewModel
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

        public ProfileViewModel()
        {
        }

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
            ImageSource picture)
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
    }
}
