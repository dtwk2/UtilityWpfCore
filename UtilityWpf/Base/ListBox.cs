using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Controls
{
    /// <summary>
    /// Generic Selector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Selector<T> : Selector where T : DependencyObject, new()
    {
        protected override DependencyObject GetContainerForItemOverride() => new T();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            InitialiseItem(element as T, item);
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is T;

        protected virtual T InitialiseItem(T item, object viewmModel) => item;
    }

    /// <summary>
    /// Generic ListBox
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListBox<T> : ListBox where T : DependencyObject, new()
    {
        protected override DependencyObject GetContainerForItemOverride() => new T();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            InitialiseItem(element as T, item);
            base.PrepareContainerForItemOverride(element, item);
        }


        protected override bool IsItemItsOwnContainerOverride(object item) => item is T;

        protected virtual T InitialiseItem(T item, object viewmModel) => item;
    }

    /// <summary>
    /// Generic ComboBox
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComboBox<T> : ComboBox where T : DependencyObject, new()
    {
        protected override DependencyObject GetContainerForItemOverride() => new T();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            InitialiseItem(element as T, item);
            base.PrepareContainerForItemOverride(element, item);
        }


        protected override bool IsItemItsOwnContainerOverride(object item) => item is T;

        protected virtual T InitialiseItem(T item, object viewmModel) => item;
    }
}
