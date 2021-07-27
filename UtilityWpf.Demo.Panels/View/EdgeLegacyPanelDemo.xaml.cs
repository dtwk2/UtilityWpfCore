﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.PanelDemo
{
    /// <summary>
    /// Interaction logic for DemoCirclePanel.xaml
    /// </summary>
    public partial class EdgeLegacyPanelDemo : UserControl
    {
        private readonly Array array;
        int i;

        public EdgeLegacyPanelDemo()
        {
            InitializeComponent();
            array = this.Resources["Array"] as Array;
            foreach(var item in array)
            {
                CirclePanel1.Children.Add(item as UIElement);
                i++;
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (i < array.Length)
            {
                CirclePanel1.Children.Add(array.GetValue(i++) as UIElement);
            }
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            CirclePanel1.Children.Remove(array.GetValue(--i) as UIElement);
           // CirclePanel1.AddLine();
        }
    }
}
