using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using UappUI.AppCode.Touch;
using static System.Math;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for ScrollIntoViewUserControl.xaml
    /// </summary>
    public partial class ScrollUserControl : UserControl
    {
        public ScrollUserControl()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer))
                return;

            const double MIN_SCALE = 0.0;
            const double MAX_SCALE = PI;
            var viewHeight = scrollViewer.ActualHeight;
            var itemMaxHeight = 100;

            List<FrameworkElement> visibleItemes = GetVisibleItems();

            foreach (FrameworkElement image in visibleItemes)
            {
                double currentPosition = GetElementPosition(image, scrollViewer.Parent as FrameworkElement).Y + image.ActualHeight / 2;
                double mappedHeightValue = currentPosition.Map(
                    itemMaxHeight * (-1), viewHeight + itemMaxHeight, MIN_SCALE, MAX_SCALE);

                var scale = Sin(mappedHeightValue);

                DoubleAnimation heightAnimation = new DoubleAnimation
                    (image.ActualHeight, itemMaxHeight * scale,
                    TimeSpan.FromMilliseconds(10), FillBehavior.HoldEnd);
                image.BeginAnimation(HeightProperty, heightAnimation);
            }

            Point GetElementPosition(FrameworkElement childElement, FrameworkElement absoluteElement)
            {
                return childElement.TransformToAncestor(absoluteElement).Transform(new Point(0, 0));
            }

            List<FrameworkElement> GetVisibleItems()
            {
                StackPanel container = (StackPanel)scrollViewer.Content;

                List<FrameworkElement> visibleItems = new List<FrameworkElement>();
                foreach (FrameworkElement item in container.Children)
                    if (IsItemVisible(item, scrollViewer))
                        visibleItems.Add(item);

                return visibleItems;

                bool IsItemVisible(FrameworkElement child, FrameworkElement parent)
                {
                    var childTransform = child.TransformToAncestor(scrollViewer);
                    var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
                    var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
                    return ownerRectangle.IntersectsWith(childRectangle);
                }
            }
        }
    }
}