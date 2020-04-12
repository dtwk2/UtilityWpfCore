using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Behavior
{
    public class DataGridVisibleItemsBehavior : Behavior<DataGrid>
    {
        private double scrollPosition;

        public int FirstIndex
        {
            get { return (int)GetValue(FirstIndexProperty); }
            set { SetValue(FirstIndexProperty, value); }
        }

        public static readonly DependencyProperty FirstIndexProperty = DependencyProperty.Register("FirstIndex", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(0));


        public int LastIndex
        {
            get { return (int)GetValue(LastIndexProperty); }
            set { SetValue(LastIndexProperty, value); }
        }

        public static readonly DependencyProperty LastIndexProperty = DependencyProperty.Register("LastIndex", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(0));


        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(0));




        public int MouseFactor
        {
            get { return (int)GetValue(MouseFactorProperty); }
            set { SetValue(MouseFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseFactorProperty = DependencyProperty.Register("MouseFactor", typeof(int), typeof(DataGridVisibleItemsBehavior), new PropertyMetadata(3));



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
                scrollViewer.ScrollChanged += AssociatedObject_ScrollChanged;

                scrollViewer
                    .ScrollChanges()
                    //.Select(a => a.VerticalChange * 2.5)
                    .Select(a => ScrollViewerOnScrollChanged(scrollViewer, dataGrid, a))
                    .Where(a => a.HasValue)
                    .Select(a => a.Value)
                         .ObserveOnDispatcher()
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

                scrollPosition = scrollViewer.VerticalOffset + e.VerticalChange * MouseFactor;// steps * stepSize;
                if (scrollPosition >= scrollViewer.ScrollableHeight)
                {
                    scrollViewer.ScrollToBottom();
                    return;
                }
                scrollViewer.ScrollToVerticalOffset(scrollPosition);
            }


            static (int, int)? ScrollViewerOnScrollChanged(ScrollViewer scrollViewer, DataGrid dataGrid, ScrollChangedEventArgs a)
            {
                //AssociatedObject_ScrollChanged();

                if (VisualTreeHelperEx.FindVisualChildren<ScrollBar>(scrollViewer).FirstOrDefault(s => s.Orientation == Orientation.Vertical) is ScrollBar verticalScrollBar)
                {
                    int totalCount = dataGrid.Items.Count;
                    var firstVisible = (verticalScrollBar.Value);
                    var lastVisible = (firstVisible + totalCount - verticalScrollBar.Maximum);

                    return ((int)firstVisible, (int)lastVisible);
                    // GetVisibibleItems(firstVisible, lastVisible, dataGrid).ToArray();
                }
                return null;

                static IEnumerable<object> GetVisibibleItems(int firstVisible, int lastVisible, DataGrid dataGrid)
                {
                    for (int i = firstVisible; i <= lastVisible; i++)
                    {
                        yield return dataGrid.Items[i];
                    }
                }
            }
        }
    }
}