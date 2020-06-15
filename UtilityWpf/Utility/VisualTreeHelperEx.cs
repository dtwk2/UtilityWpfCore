using System.Collections.Generic;
using System.Windows;
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
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                  
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}