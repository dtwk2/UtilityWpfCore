using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.View
{
    /// <summary>
    /// Interaction logic for HostUserControl.xaml
    /// </summary>
    public partial class HostUserControl : UserControl
    {
        public HostUserControl()
        {
            // UserControls 
            InitializeComponent();
            this.DockPanel1.DataContext = this;
            UserControls = Assembly.GetEntryAssembly().GetTypes()
                .Where(a => typeof(UserControl).IsAssignableFrom(a))
                .Select(a => (UserControl)Activator.CreateInstance(a))
                .GroupBy(a => string.IsNullOrEmpty(a.Name) ?
               a.GetType().Name.Replace("UserControl", string.Empty) :
                 a.Name)
                .OrderBy(a => a.Key)
                .ToDictionary();

            ItemsControl1.ItemsSource = UserControls;
            ContentControl1.Content = UserControls.FirstOrDefault().Value;
        }

        public IDictionary<string, UserControl> UserControls { get; }
    }


    public static class sa
    {
        public static Dictionary<string, T> ToDictionary<T>(this IEnumerable<IGrouping<string, T>> groupings)
            =>
      groupings
           .Select(a => a.Select((b, i) => (b, i))
          .ToDictionary(c =>
          c.i > 0 ? a.Key + c.i : a.Key, c => c.b))
          .SelectMany(a => a)
          .ToDictionary(a => a.Key, a => a.Value);


    }
}
