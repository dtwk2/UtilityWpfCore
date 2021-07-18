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

            UserControls = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(a => typeof(UserControl).IsAssignableFrom(a))
                .GroupBy(a => a.Name?.Replace("UserControl", string.Empty))
                .OrderBy(a => a.Key)
                .ToDictionaryOnIndex()
                .ToDictionary(a => a.Key, a => new Func<UserControl>(() => (UserControl)Activator.CreateInstance(a.Value)));

            MainListBox.ItemsSource = UserControls;
            ContentControl1.Content = UserControls.FirstOrDefault().Value.Invoke();

            MainListBox.SelectAddChanges().Subscribe(async =>
            {
                ContentControl1.Content = ((KeyValuePair<string, Func<UserControl>>)MainListBox.SelectedItem).Value.Invoke();
            });
        }
   
        public IDictionary<string, Func<UserControl>> UserControls { get; }
    }

    public static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
           .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
          .ToDictionary(a => a.Key, a => a.Value);
    }
}