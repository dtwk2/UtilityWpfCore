using System;
using System.Collections.Generic;
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

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for CompareSliderUserControl.xaml
    /// </summary>
    public partial class CompareSliderUserControl : UserControl
    {
        public CompareSliderUserControl()
        {
            InitializeComponent();


        }

        private void btnSlideUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BeginStoryboard((Storyboard)this.FindResource("SlideUpAnimation"));
        }

        private void btnSlideDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BeginStoryboard((Storyboard)this.FindResource("SlideDownAnimation"));
        }
    }
}
