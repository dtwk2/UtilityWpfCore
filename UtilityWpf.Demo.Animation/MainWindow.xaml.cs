﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using UtilityWpf.Demo.Animation.View;

namespace UtilityWpf.DemoAnimation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.HostUserControl.Assembly = typeof(BarUserControl).Assembly;
        }
    }
}