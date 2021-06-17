using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UtilityWpf.Behavior
{
    /// <summary>
    /// https://stackoverflow.com/questions/34427456/retrieve-list-of-visible-items-in-datagrid
    /// </summary>
    public class DataGridExtensions
    {
        public static readonly DependencyProperty ObserveVisiblePersonsProperty = DependencyProperty.RegisterAttached(
            "ObserveVisiblePersons", typeof(bool), typeof(DataGridExtensions),
            new PropertyMetadata(false, OnObserveVisiblePersonsChanged));

        public static readonly DependencyProperty VisibleItemsProperty = DependencyProperty.RegisterAttached(
            "VisibleItems", typeof(IEnumerable), typeof(DataGridExtensions),
            new PropertyMetadata(null));

        private static readonly DependencyProperty SenderDataGridProperty = DependencyProperty.RegisterAttached(
            "SenderDataGrid", typeof(DataGrid), typeof(DataGridExtensions), new PropertyMetadata(default(DataGrid)));

        public static void SetObserveVisiblePersons(DependencyObject element, bool value) => element.SetValue(ObserveVisiblePersonsProperty, value);

        public static bool GetObserveVisiblePersons(DependencyObject element) => (bool)element.GetValue(ObserveVisiblePersonsProperty);

        private static void SetSenderDataGrid(DependencyObject element, DataGrid value) => element.SetValue(SenderDataGridProperty, value);

        private static DataGrid GetSenderDataGrid(DependencyObject element) => (DataGrid)element.GetValue(SenderDataGridProperty);

        public static void SetVisibleItems(DependencyObject element, object[] value) => element.SetValue(VisibleItemsProperty, value);

        public static IEnumerable GetVisibleItems(DependencyObject element) => (IEnumerable)element.GetValue(VisibleItemsProperty);

        private static void OnObserveVisiblePersonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
                dataGrid.Loaded += (sender, _) => DataGridLoaded(sender as DataGrid);
        }

        private static void DataGridLoaded(DataGrid dataGrid)
        {
            if (VisualTreeHelperEx.FindChildren<ScrollViewer>(dataGrid).FirstOrDefault() is ScrollViewer scrollViewer)
            {
                SetSenderDataGrid(scrollViewer, dataGrid);
                scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
            }
        }

        private static void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
                if (VisualTreeHelperEx.FindChildren<ScrollBar>(scrollViewer).FirstOrDefault(s => s.Orientation == Orientation.Vertical) is ScrollBar verticalScrollBar)
                {
                    DataGrid dataGrid = GetSenderDataGrid(scrollViewer);

                    int totalCount = dataGrid.Items.Count;
                    int firstVisible = (int)verticalScrollBar.Value;
                    int lastVisible = (int)(firstVisible + totalCount - verticalScrollBar.Maximum);

                    SetVisibleItems(dataGrid, GetVisibibleItems(firstVisible, lastVisible, dataGrid).ToArray());
                }

            static IEnumerable<object> GetVisibibleItems(int firstVisible, int lastVisible, DataGrid dataGrid)
            {
                for (int i = firstVisible; i <= lastVisible; i++)
                {
                    yield return dataGrid.Items[i];
                }
            }
        }

        private static class VisualTreeHelperEx
        {
            public static IList<T> FindChildren<T>(DependencyObject element) where T : FrameworkElement
            {
                List<T> retval = new List<T>();
                for (int counter = 0; counter < VisualTreeHelper.GetChildrenCount(element); counter++)
                {
                    FrameworkElement toadd = VisualTreeHelper.GetChild(element, counter) as FrameworkElement;
                    if (toadd != null)
                    {
                        T correctlyTyped = toadd as T;
                        if (correctlyTyped != null)
                        {
                            retval.Add(correctlyTyped);
                        }
                        else
                        {
                            retval.AddRange(FindChildren<T>(toadd));
                        }
                    }
                }
                return retval;
            }
        }
    }
}