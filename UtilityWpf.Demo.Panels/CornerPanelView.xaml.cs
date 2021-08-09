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

namespace UtilityWpf.Demo.Panels.View
{
    /// <summary>
    /// Interaction logic for DemoCornerPanel.xaml
    /// </summary>
    public partial class CornerPanelView : UserControl
    {
        public CornerPanelView()
        {
            InitializeComponent();
        }

        private void ShowButton_Checked(object sender, RoutedEventArgs e)
        {
            CornerPanelControl.ShowLine();
            CornerPanelControl2.ShowLine();
        }

        private void AddToCollection(object sender, RoutedEventArgs e)
        {
            DemoViewModel.Instance.Collection.Add("c");

        }
    }
}
