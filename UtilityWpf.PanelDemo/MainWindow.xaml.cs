using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace  UtilityWpf.PanelDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {      
            InitializeComponent();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AddToCollection(object sender, RoutedEventArgs e)
        {
            DemoViewModel.Instance.Collection.Add("d");
        }
    }


    public class DemoViewModel
    {
        public DemoViewModel()
        {

        }

        public ObservableCollection<string> Collection { get; } = new ObservableCollection<string>(Enumerable.Range(0, 10).Select(a => a.ToString()));
        public string[] HalfCollection { get; } = Enumerable.Range(0, 4).Select(a => a.ToString()).ToArray();

        public static DemoViewModel Instance { get; } = new DemoViewModel();
    }
}
