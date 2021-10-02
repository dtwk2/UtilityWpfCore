using System;
using System.Collections.Generic;
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
using UtilityHelper.NonGeneric;

namespace UtilityWpf.Demo.Extrinsic
{
    /// <summary>
    /// Interaction logic for AnimatedListBoxUserControl.xaml
    /// </summary>
    public partial class AnimatedListBoxUserControl : UserControl
    {
        Random random = new Random();   
        public AnimatedListBoxUserControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AnimatedListBox.ScrollToSelectedItem = true;
            var index = random.Next(0, AnimatedListBox.ItemsSource.Count() - 1);
            AnimatedListBox.SelectedItem = AnimatedListBox.ItemsSource.Cast<object>().ElementAt(index);
        }

    }
}
