using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace UtilityWpf.Behavior
{
    public class DataGridVisibleItemsBehavior : Behavior<DataGrid>
    {
        private double scrollPosition;

        public static readonly DependencyProperty FirstIndexProperty = DependencyProperty.Register("FirstIndex", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty LastIndexProperty = DependencyProperty.Register("LastIndex", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty MouseFactorProperty = DependencyProperty.Register("MouseFactor", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(3));

        public int FirstIndex
        {
            get { return (int)GetValue(FirstIndexProperty); }
            set { SetValue(FirstIndexProperty, value); }
        }


        public int LastIndex
        {
            get { return (int)GetValue(LastIndexProperty); }
            set { SetValue(LastIndexProperty, value); }
        }


        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }


        public int MouseFactor
        {
            get { return (int)GetValue(MouseFactorProperty); }
            set { SetValue(MouseFactorProperty, value); }
        }


        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += (sender, _) => DataGridLoaded(sender as DataGrid);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                AssociatedObject.Loaded -= (sender, _) => DataGridLoaded(sender as DataGrid);
            }
        }

        private void DataGridLoaded(DataGrid dataGrid)
        {
            if (VisualTreeHelperEx.FindVisualChildren<ScrollViewer>(dataGrid).FirstOrDefault() is ScrollViewer scrollViewer)
            {
                // N.B this doesn't work well if VerticalScrollBar is used to scroll- works for mouse-wheel.
                if (MouseFactor > 1)
                    scrollViewer.ScrollChanged += AssociatedObject_ScrollChanged;
                scrollViewer
                    .ScrollChanges()
                         .Select(a => ScrollViewerOnScrollChanged(scrollViewer, dataGrid, a))
                    .Where(a => a.HasValue)
                    .Select(a => a.Value)
                    .Subscribe(a =>
                {
                    var (firstVisible, lastVisible) = a;
                    FirstIndex = firstVisible;
                    LastIndex = lastVisible;
                    Size = (lastVisible - firstVisible) + 1;
                });
            }

            void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                if (scrollViewer.VerticalOffset == scrollPosition)
                    return;

                scrollPosition = scrollViewer.VerticalOffset + e.VerticalChange * MouseFactor;
                if (scrollPosition >= scrollViewer.ScrollableHeight)
                    scrollViewer.ScrollToBottom();
                else
                    scrollViewer.ScrollToVerticalOffset(scrollPosition);
            }

            static (int, int)? ScrollViewerOnScrollChanged(ScrollViewer scrollViewer, DataGrid dataGrid, ScrollChangedEventArgs a)
            {
                //AssociatedObject_ScrollChanged();

                if (VisualTreeHelperEx.FindVisualChildren<ScrollBar>(scrollViewer).Single(s => s.Orientation == Orientation.Vertical) is ScrollBar verticalScrollBar)
                {
                    int totalCount = dataGrid.Items.Count;
                    var firstVisible = (verticalScrollBar.Value);
                    var lastVisible = (firstVisible + totalCount - verticalScrollBar.Maximum);

                    return ((int)firstVisible, (int)lastVisible);
                }
                return null;

            }
        }
    }
}