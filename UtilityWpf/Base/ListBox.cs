﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Base
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
    public class ListBox<T> : LayOutListBox where T : DependencyObject, new()
    {
        protected override DependencyObject GetContainerForItemOverride() => new T();

        protected sealed override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not T t)
                throw new System.Exception("s fdsd  77ffs");
            InitialiseItem(t, item);
            PrepareContainerForItemOverride(t, item);
            base.PrepareContainerForItemOverride(element, item);
        }

        protected virtual void PrepareContainerForItemOverride(T element, object item)
        {
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is T;

        protected virtual T InitialiseItem(T item, object viewmModel)
        {
            if (item is FrameworkElement control)
                _ = control.ApplyTemplate();
            return item;
        }
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