using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using NetFabric.Hyperlinq;
using UappUI.AppCode.Touch;
using UtilityHelper.NonGeneric;
using static System.Math;

namespace UtilityWpf.DemoApp {
   /// <summary>
   /// Interaction logic for ScrollIntoViewUserControl.xaml
   /// </summary>
   public partial class ScrollUserControl : UserControl {
      private Random random = new();
      public ScrollUserControl() {
         InitializeComponent();


      }

      private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
         if (!(sender is ScrollViewer scrollViewer))
            return;

         const double MIN_SCALE = 0.0;
         const double MAX_SCALE = PI;
         var viewHeight = scrollViewer.ActualHeight;
         var itemMaxHeight = 100;

         foreach (FrameworkElement image in GetVisibleItems()) {
            double currentPosition = GetElementPosition(image, scrollViewer.Parent as FrameworkElement).Y + image.ActualHeight / 2;
            double mappedHeightValue = currentPosition.Map(
                itemMaxHeight * (-1), viewHeight + itemMaxHeight, MIN_SCALE, MAX_SCALE);

            var scale = Sin(mappedHeightValue);

            DoubleAnimation heightAnimation = new DoubleAnimation
                (image.ActualHeight, itemMaxHeight * scale,
                TimeSpan.FromMilliseconds(10), FillBehavior.HoldEnd);
            image.BeginAnimation(HeightProperty, heightAnimation);
         }

         Point GetElementPosition(FrameworkElement childElement, FrameworkElement absoluteElement) {
            return childElement.TransformToAncestor(absoluteElement).Transform(new Point(0, 0));
         }

         IEnumerable<FrameworkElement> GetVisibleItems() {
            StackPanel container = (StackPanel)scrollViewer.Content;

            foreach (FrameworkElement item in container.Children)
               if (IsItemVisible(item, scrollViewer))
                  yield return (item);


            bool IsItemVisible(FrameworkElement child, FrameworkElement parent) {
               var childTransform = child.TransformToAncestor(scrollViewer);
               var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
               var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
               return ownerRectangle.IntersectsWith(childRectangle);
            }
         }
      }

      private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
      {
         AnimatedListBox.ScrollToSelectedItem = true;
         var index = random.Next(0, AnimatedListBox.ItemsSource.Count() - 1);
         AnimatedListBox.SelectedItem = AnimatedListBox.ItemsSource.Cast<object>().ElementAt(index);
      }

      private void ButtonBase_OnClick1(object sender, RoutedEventArgs e) {

         ListView1.SelectedIndex = random.Next(0, ListView1.ItemsSource.Count() - 1);
      }
   }
}