using System.Windows;
using System.Windows.Documents;
using UtilityWpf.Adorners;

namespace UtilityWpf.Utility
{
    public static class AdornerHelper
    {
        public static void AddIfMissingAdorner(this DependencyObject adornedElement, DependencyObject adorner)
        {
            AdornerCollection? adorners = AdornerEx.GetAdorners(adornedElement);
            if (adorners.IndexOf(adorner) == -1)
            {
                adorners.Add(adorner);
            }
        }

        public static void RemoveAdorners(this UIElement adornedElement)
        {
            AdornerLayer? layer = AdornerLayer.GetAdornerLayer(adornedElement);
            Adorner[]? toRemoveArray = layer.GetAdorners(adornedElement);
            if (toRemoveArray != null)
            {
                foreach (Adorner a in toRemoveArray)
                {
                    layer.Remove(a);
                }
            }
        }
    }
}