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
            InitializeComponent();
            this.DataContext = new Faker<Data>().Rules((f,a) => new Data
            {
                A = f.Date.Future().ToString("F"),
                B = f.Company.CatchPhrase(),
                C = f.Phone.PhoneNumber(),
                D = f.Name.LastName()
            }).Generate(100);
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