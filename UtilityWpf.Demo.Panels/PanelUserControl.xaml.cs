using System;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Demo.Panels
{
    /// <summary>
    /// Interaction logic for PanelUserControl.xaml
    /// </summary>
    public partial class PanelUserControl : UserControl
    {
        public PanelUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ContentControl cc && ListBox1.SelectedIndex == -1)
            {
                char[] x = { 'A', 'B', 'C', 'D', 'E' };

                ListBox1.SelectedIndex = Array.IndexOf(x, cc.Content.ToString()[0]);
                return;
            }
            ListBox1.SelectedIndex = -1;
        }
    }
}
