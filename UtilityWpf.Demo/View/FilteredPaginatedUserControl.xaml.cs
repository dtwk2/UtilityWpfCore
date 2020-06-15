using Bogus;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for FilteredPaginatedUserControl.xaml
    /// </summary>
    public partial class FilteredPaginatedUserControl : UserControl
    {
        public FilteredPaginatedUserControl()
        {

            var fk = new Faker<Data>()
                .RuleFor(a => a.A, f => f.Date.Future().ToString("F"))
                                  .RuleFor(a => a.B, f => f.Company.CatchPhrase())
                                                 .RuleFor(a => a.C, f => f.Phone.PhoneNumber())
                                                                .RuleFor(a => a.D, f => f.Name.LastName());
            InitializeComponent();

            var items = fk.Generate(100);
            this.DataContext = items;

        }

        private class Data
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
        }
    }
}