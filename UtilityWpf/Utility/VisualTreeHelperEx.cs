﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilityWpf
{

    /// <summary>
    /// https://github.com/Rudnicky/VisualTreeHelper/blob/master/VisualTreeTraversionPoC/Utils/VisualTreeTraverseHelper.cs
    /// </summary>
    public static class VisualTreeHelperEx
    {

    //https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type/978352#978352
    //answered Jun 10 '09 at 21:53
    //Bryce Kahle
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

            /// <summary>
            /// Finds a Parent of given control
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="child"></param>
            /// <returns></returns>
            public static object? FindParent(this DependencyObject child, Type type) =>
                VisualTreeHelper.GetParent(child) switch
                {
                    null => null,
                    { } parent when parent.GetType() == type => parent,
                    { } parent => parent.FindParent(type)
                };

            /// <summary>
            /// Finds a Parent of given control
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="child"></param>
            /// <returns></returns>
            public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject =>
                VisualTreeHelper.GetParent(child) switch
                {
                    null => null,
                    T parent => parent,
                    { } parent => FindParent<T>(parent)
                };

            /// <summary>
            /// Finds parent by given name
            /// </summary>
            /// <param name="dependencyObject"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public static DependencyObject? FindParent(this DependencyObject dependencyObject, string name)
            {
                while (dependencyObject != null &&
                       VisualTreeHelper.GetParent(dependencyObject) is { } parentObj)
                {
                    if ((string)parentObj.GetValue(FrameworkElement.NameProperty) == name)
                        return parentObj;

                    dependencyObject = parentObj;
                }
                return null;
            }

            /// <summary>
            /// Finds a Child of a given item in the visual tree by name
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="parent"></param>
            /// <param name="childName"></param>
            /// <returns></returns>
            public static T? FindChild<T>(this DependencyObject parent, string? childName = null) where T : DependencyObject
            {
                // Confirm parent and childName are valid.
                if (parent == null) return null;

                T? foundChild = null;

                int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    // If the child is not of the request child type child
                    if (!(child is T t))
                    {
                        // recursively drill down the tree
                        foundChild = FindChild<T>(child, childName);

                        // If the child is found, break so we do not overwrite the found child.
                        if (foundChild != null) break;
                    }
                    else if (!string.IsNullOrEmpty(childName))
                    {
                        // If the child's name is set for search
                        if (!(child is FrameworkElement frameworkElement) || frameworkElement.Name != childName) continue;
                        // if the child's name is of the request name
                        foundChild = t;
                        break;
                    }
                    else
                    {
                        // child element found.
                        foundChild = t;
                        break;
                    }
                }

                return foundChild;
            }

            /// <summary>
            /// Finds all controls of a specific type in visual tree
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="parent"></param>
            /// <returns></returns>
            public static IEnumerable<T> FindChildren<T>(this DependencyObject parent) where T : DependencyObject
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    if (child is T childType)
                    {
                        yield return childType;
                    }

                    foreach (var other in FindChildren<T>(child))
                    {
                        yield return other;
                    }
                }
            }

            /// <summary>
            /// Gets child of specific type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="depObj"></param>
            /// <returns></returns>
            public static T? ChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
            {
                if (depObj == null) return null;

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);

                    var result = child as T ?? ChildOfType<T>(child);
                    if (result != null) return result;
                }
                return null;
            }
        }
    }