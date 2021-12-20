using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using UtilityWpf.Controls.Progress;

namespace UtilityWpf.Demo.Progress
{
    /// <summary>
    /// Interaction logic for GaugeTwoUserControl.xaml
    /// </summary>
    public partial class CircleProgressUserControl : UserControl
    {
        public CircleProgressUserControl()
        {
            InitializeComponent();

            Observable
                .Interval(TimeSpan.FromSeconds(0.1))
                .ObserveOnDispatcher()
                .Subscribe(a =>
                {
                    ProgressCircle.Value = a % 90;
                });
        }
    }
}