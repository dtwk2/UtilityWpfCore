using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.PanelDemo
{
    /// <summary>
    /// Interaction logic for DemoSidePanel.xaml
    /// </summary>
    public partial class DemoSidePanel : UserControl
    {
        public DemoSidePanel()
        {
            InitializeComponent();
        }

        private void AddToCollection(object sender, RoutedEventArgs e)
        {
            DemoViewModel.Instance.Collection.Add("s");
        }
    }
}
