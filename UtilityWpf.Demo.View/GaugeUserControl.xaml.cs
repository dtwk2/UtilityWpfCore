using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilityWpf.Demo.View.Animation
{
    /// <summary>
    /// Interaction logic for Gauge2UserControl.xaml
    /// </summary>
    public partial class GaugeUserControl : UserControl
    {
        public GaugeUserControl()
        {
            InitializeComponent();

            Observable
   .Interval(TimeSpan.FromSeconds(0.1))
   .ObserveOnDispatcher()
   .Subscribe(a =>
   {
       circProg.Value = a % 90;
   });
        }
    }

}