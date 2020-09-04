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
using UtilityWpf.Interactive.Demo.Common;

namespace UtilityWpf.Interactive.Demo
{
    /// <summary>
    /// Interaction logic for ViewModelAssemblyView.xaml
    /// </summary>
    public partial class ViewModelAssemblyView : UserControl
    {
        public ViewModelAssemblyView()
        {
            InitializeComponent();

            Init();
        }

        async void Init()
        {
            Main_MasterDetailView.ItemsSource = await new ViewModelAssemblyModel().Collection;
        }
    }
}
