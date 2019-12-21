using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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

namespace UtilityWpf.DemoAppCore.View
{
    /// <summary>
    /// Interaction logic for TimeBarUserControl.xaml
    /// </summary>
    public partial class TimeBarUserControl : UserControl
    {
        public TimeBarUserControl()
        {
            InitializeComponent();

            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Scan(DateTime.Now, (a, b) => a + TimeSpan.FromHours(b))
                .ObserveOnDispatcher()
                .Subscribe(time => TimeBar1.SelectedTime = time);
        }
    }
}
