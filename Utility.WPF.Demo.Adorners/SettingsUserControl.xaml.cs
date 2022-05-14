﻿using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Demo.Adorners.Infrastructure;
using UtilityWpf.Controls.Meta;
using UtilityWpf.Demo.Data.Factory;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        private readonly AdornerController adornerController;
        private readonly ControlColourer controlColourer;
        private bool flag, flag2;

        public SettingsUserControl()
        {
            InitializeComponent();

            controlColourer = new(MainCanvas);
            adornerController = new(this);

            Square3Grid.DataContext = DataContexts.Random;
            SettingsAdorner.AddTo(Square3Grid);

            GearGrid.SetValue(AdornerEx.AdornerProperty, new CustomFrameworkElementAdorner(GearGrid));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
                controlColourer.Remove();
            else
                controlColourer.Apply();
            flag = !flag;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (flag2)            
                adornerController?.Hide();
            else            
                adornerController?.Apply();            
            flag2 = !flag2;
        }
    }
}
