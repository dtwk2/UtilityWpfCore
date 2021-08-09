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

namespace UtilityWpf.Demo.View.Panels
{
    /// <summary>
    /// Interaction logic for DemoUniformGridPanel.xaml
    /// </summary>
    public partial class UniformGridPanelView : UserControl
    {
        Random random = new Random();

        public UniformGridPanelView()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var width = random.Next(50, 300);
            var height = random.Next(50, 300);
            this.UniformGridPanel1.Children.Add(new Ellipse { Fill = Brushes.Teal, Width = width, Height = height });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
