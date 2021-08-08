using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Demo.Animation.View
{
    /// <summary>
    /// Interaction logic for FadeUserControl.xaml
    /// </summary>
    public partial class FadeUserControl : UserControl
    {
        public FadeUserControl()
        {
            InitializeComponent();

            var interval = Observable
                .Interval(TimeSpan.FromSeconds(2));

            var progress = interval.Select(a => Observable
                                                        .Interval(TimeSpan.FromSeconds(0.1))
                                                        .Scan(0, (a, b) => ++a)
                                                        .Select(a => a * 100 / (2 / 0.1)))
                .Switch()
                .ObserveOnDispatcher()
                .Subscribe(a =>
                {
                    MainProgressBar.Value = a;
                });

            interval.ObserveOnDispatcher().Subscribe(a =>
            {
                MainFadeControl.FadeCommand.Execute(default);
            });

        }
    }
}
