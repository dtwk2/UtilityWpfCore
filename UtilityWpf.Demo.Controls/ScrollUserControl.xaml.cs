﻿using NetFabric.Hyperlinq;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper.NonGeneric;

namespace UtilityWpf.Demo.Controls
{
    /// <summary>
    /// Interaction logic for ScrollIntoViewUserControl.xaml
    /// </summary>
    public partial class ScrollUserControl : UserControl
    {
        private Random random = new();

        public ScrollUserControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            ListView1.SelectedIndex = random.Next(0, ListView1.ItemsSource.Count() - 1);
        }
    }
}