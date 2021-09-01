using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    /// <summary>
    /// Generic ListBox
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListBox<T> : ListBox where T : DependencyObject, new()
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new T();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            InitialiseItem(element as T, item);
            base.PrepareContainerForItemOverride(element, item);
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is T;
        }

        protected virtual T InitialiseItem(T item, object viewmModel)
        {
            return item;
        }
    }

}
