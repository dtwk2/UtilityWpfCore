﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UniversalWPF;

namespace UtilityWpf.Demo.SandBox
{
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

    public class NumberItem : ListBoxItem
    {
        static NumberItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberItem), new FrameworkPropertyMetadata(typeof(NumberItem)));
        }
    }

    internal class NumbersControl : ListBox<NumberItem>
    {
        public static readonly DependencyProperty DisplayKeyPathProperty = DependencyProperty.Register("DisplayKeyPath", typeof(string), typeof(NumbersControl), new PropertyMetadata(null));

        static NumbersControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NumbersControl), new FrameworkPropertyMetadata(typeof(NumbersControl)));
        }

        public string DisplayKeyPath
        {
            get { return (string)GetValue(DisplayKeyPathProperty); }
            set { SetValue(DisplayKeyPathProperty, value); }
        }

        protected override NumberItem InitialiseItem(NumberItem item, object viewModel)
        {
            if (item is not Control control)
                return item;
            if (VisualTreeHelper.GetChildrenCount(control) == 0)
                _ = control.ApplyTemplate();

            SetTextBinding(item, viewModel);
            SetValueBinding(item, viewModel);

            return item;
        }

        private void SetValueBinding(NumberItem item, object viewModel)
        {
            if (string.IsNullOrEmpty(DisplayKeyPath))
                return;
            if (item.ChildOfType<TextBlock>() is not TextBlock textBlock)
                return;

            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, CreateKeyBinding(viewModel));

            Binding CreateKeyBinding(object item)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayKeyPath),
                    Mode = BindingMode.OneTime
                };
            }
        }

        private void SetTextBinding(NumberItem item, object viewModel)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;

            if (item.ChildOfType<NumberBox>() is not NumberBox numberBox)
                return;

            BindingOperations.SetBinding(numberBox, NumberBox.ValueProperty, CreateBinding(viewModel));

            Binding CreateBinding(object item)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
            }
        }
    }
}