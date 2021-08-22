namespace UtilityWpf.Behavior
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// Uses ItemsControl's internal scrollviewer to move last item into view when items collection modified.
    /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/8a745550-4360-4b53-85f5-032f8f46e2cc/listview-scrollintoview-not-working-with-a-observablecollection?forum=wpf"></a>
    /// <a href="https://stackoverflow.com/questions/16942580/why-doesnt-listview-scrollintoview-ever-work"></a>
    /// <a href="https://stackoverflow.com/questions/7153302/scroll-animation"></a>
    /// </summary>
    public class SmoothScrollToEndBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject.Items is INotifyCollectionChanged)
                ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += ListView_CollectionChanged;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject.Items is INotifyCollectionChanged)
                ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= ListView_CollectionChanged;
            base.OnDetaching();
        }

        public bool WithAnimation
        {
            get { return (bool)GetValue(WithAnimationProperty); }
            set { SetValue(WithAnimationProperty, value); }
        }

        public static readonly DependencyProperty WithAnimationProperty =
            DependencyProperty.Register("WithAnimation", typeof(bool), typeof(SmoothScrollToEndBehavior), new PropertyMetadata(true));

        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == NotifyCollectionChangedAction.Add) || e.Action == NotifyCollectionChangedAction.Remove)
            {
                try
                {
                    var scrollViewer = Helper.GetDescendantByType(this.AssociatedObject, typeof(ScrollViewer)) as ScrollViewer;
                    if (WithAnimation)
                    {
                        var storyboard = Helper.MakeScrollAnimation(scrollViewer, Helper.GetRatio(this.AssociatedObject, this.AssociatedObject.Items.Count), 20);
                        storyboard.Begin();
                    }
                    else
                        scrollViewer.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error updating list view", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private class Helper
        {
            public static Storyboard MakeScrollAnimation(ScrollViewer scrollViewer, double ratio, int offset)
            {
                double toValue = scrollViewer.ScrollableHeight * ratio;
                DoubleAnimation verticalAnimation = new DoubleAnimation();
                verticalAnimation.From = scrollViewer.VerticalOffset;
                verticalAnimation.To = toValue + offset;
                verticalAnimation.DecelerationRatio = .2;
                verticalAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(verticalAnimation);
                Storyboard.SetTarget(verticalAnimation, scrollViewer);
                Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));
                return storyboard;
            }

            public static double GetRatio(ItemsControl itemsControl, int index)
            {
                return ((double)index) / itemsControl.Items.Count;
            }

            public static Visual GetDescendantByType(Visual element, Type type)
            {
                if (element == null) return null;
                if (element.GetType() == type) return element;
                Visual foundElement = null;
                if (element is FrameworkElement)
                {
                    (element as FrameworkElement).ApplyTemplate();
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                    foundElement = GetDescendantByType(visual, type);
                    if (foundElement != null)
                        break;
                }
                return foundElement;
            }
        }
    }

    /// <summary>
    ///   Based on the following link
    ///   <a href="http://aniscrollviewer.codeplex.com/"></a>
    ///  The VerticalOffset property is read-only so instead you can use an attached property VerticalOffset on the ScrollViewer which in turn does ScrollToVerticalOffset.This attached property can be animated.
    /// </summary>
    public class ScrollViewerBehavior
    {
        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                                                typeof(double),
                                                typeof(ScrollViewerBehavior),
                                                new UIPropertyMetadata(0d, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }
    }
}