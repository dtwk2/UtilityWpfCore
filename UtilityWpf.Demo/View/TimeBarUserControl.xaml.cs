using System;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp.View
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
                .TakeWhile(a => a.Year < DateTime.MaxValue.Year)
                .ObserveOnDispatcher()
                .Subscribe(time => TimeBar1.SelectedTime = time);
        }
    }
}