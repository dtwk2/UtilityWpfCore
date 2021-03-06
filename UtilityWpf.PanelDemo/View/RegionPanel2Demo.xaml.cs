﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.PanelDemo
{
    /// <summary>
    /// Interaction logic for DemoCirclePanel.xaml
    /// </summary>
    public partial class RegionPanel2Demo : UserControl
    {
        private readonly Array array;
        int i;

        public RegionPanel2Demo()
        {
            InitializeComponent();
            array = this.Resources["Array"] as Array;
            foreach (var item in array)
            {
                CirclePanel1.Children.Add(item as UIElement);
                i++;
            }

            UseDesiredSizeCheckBox.Checked += UseDesiredSizeCheckBox_Checked;
            UseDesiredSizeCheckBox.Unchecked += UseDesiredSizeCheckBox_Checked;
        }



        private void UseDesiredSizeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CirclePanel1.UseDesiredSize = !(UseDesiredSizeCheckBox.IsChecked ?? false);
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
        }
    }
}
