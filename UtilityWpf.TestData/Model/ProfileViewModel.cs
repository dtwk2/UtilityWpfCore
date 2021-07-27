using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;

namespace UtilityWpf.TestData
{
    public class ProfileViewModel //: IEquatable<ProfileViewModel>
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
        { }

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