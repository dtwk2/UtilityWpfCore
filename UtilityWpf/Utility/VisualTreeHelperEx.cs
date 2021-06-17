using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilityWpf
{
    //also
    //LogicalTreeHelper.FindLogicalNode(DependencyObject depObj, string elementName)

    //https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type/978352#978352
    //answered Jun 10 '09 at 21:53
    //Bryce Kahle
    public static class VisualTreeHelperEx
    {
        //    public static T GetChildOfType<T>(this DependencyObject depObj)
        //        where T : DependencyObject
        //    {
        //        if (depObj == null) return null;

        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //        {
        //            var child = VisualTreeHelper.GetChild(depObj, i);

        //            var result = (child as T) ?? GetChildOfType<T>(child);
        //            if (result != null) return result;
        //        }
        //        return null;
        //    }
        //}

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static object FindItemsPanel(Visual visual)

        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)

            {
                Visual child = VisualTreeHelper.GetChild(visual, i) as Visual;

                if (child != null)

                {
                    if (child is object && VisualTreeHelper.GetParent(child) is ItemsPresenter)

                    {
                        object temp = child;

                        return (object)temp;
                    }

                    object panel = FindItemsPanel(child);

                    if (panel != null)

                    {
                        object temp = panel;

                        return (object)temp; // return the panel up the call stack
                    }
                }
            }

            return default(object);
        }
    }
}