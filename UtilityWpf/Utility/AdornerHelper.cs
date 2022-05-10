using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using HandyControl.Controls;
using UtilityWpf.Adorners;

namespace UtilityWpf.Utility
{
    public static class AdornerHelper
    {
        public static void AddAdorner(this UIElement adornedElement, DependencyObject adorner)
{
            var adorners = AdornerEx.GetAdorners(adornedElement);
            if (adorners.IndexOf(adorner) == -1)
                adorners.Add(adorner);
        }     
        
        public static void RemoveAdorners(this UIElement adornedElement)
        {
            var layer = AdornerLayer.GetAdornerLayer(adornedElement);
            var toRemoveArray = layer.GetAdorners(adornedElement);
            if (toRemoveArray != null)
                foreach (Adorner a in toRemoveArray)
                {
                    layer.Remove(a);
                }
        }
    }
}
