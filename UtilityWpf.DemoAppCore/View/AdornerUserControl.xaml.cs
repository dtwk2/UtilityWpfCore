using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.DemoAppCore.View
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class AdornerUser : UserControl
    {
        public AdornerUser()
        {
            InitializeComponent();
            TextCommand = new Command(() => TextBlock1.Text = TextBlock1.Text + " New Text");
            Grid1.DataContext = this;
        }

        public ICommand TextCommand { get; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock1.Text.Length >= 9)
                TextBlock1.Text = TextBlock1.Text.Remove(TextBlock1.Text.Length - 9);
        }
    }

}
