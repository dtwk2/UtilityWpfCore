﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    /// <summary>
    /// Interaction logic for PersistListUserControl.xaml
    /// </summary>
    public partial class ReadOnlyMasterDetailUserControl : UserControl
    {
        public ReadOnlyMasterDetailUserControl()
        {
            InitializeComponent();
            //(this.DataContext as PersistListViewModel).Data = PersistBehavior.Items;
        }

        private void DragablzItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void dfsdsf(object sender, RoutedEventArgs e)
        {

        }
    }
}