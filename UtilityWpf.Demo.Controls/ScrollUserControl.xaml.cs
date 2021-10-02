using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using NetFabric.Hyperlinq;

using UtilityHelper.NonGeneric;
using static System.Math;

namespace UtilityWpf.Demo.Controls {
   /// <summary>
   /// Interaction logic for ScrollIntoViewUserControl.xaml
   /// </summary>
   public partial class ScrollUserControl : UserControl {
      private Random random = new();
      public ScrollUserControl() {
         InitializeComponent();


      }

  


      private void ButtonBase_OnClick1(object sender, RoutedEventArgs e) {

         ListView1.SelectedIndex = random.Next(0, ListView1.ItemsSource.Count() - 1);
      }


    }
}