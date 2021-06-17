using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Windows.Controls;

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
                .GroupBy(a => string.IsNullOrEmpty(a.Name) ? a.GetType().Name.Replace("UserControl", string.Empty) : a.Name)
                .OrderBy(a => a.Key)
                .ToDictionaryOnIndex();

            MainListBox.ItemsSource = UserControls;
            ContentControl1.Content = UserControls.FirstOrDefault().Value;

            MainListBox.SelectionChanged += ItemsControl1_SelectionChanged;
        }

        private void ItemsControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContentControl1.Content = ((KeyValuePair<string, UserControl>)MainListBox.SelectedItem).Value;
        }

        public IDictionary<string, UserControl> UserControls { get; }
    }

    public static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
           .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
          .ToDictionary(a => a.Key, a => a.Value);
    }
}