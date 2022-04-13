﻿using Fasterflect;
using ReactiveUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Base;
using UtilityWpf.Utility;

namespace UtilityWpf.Controls.Buttons
{
    public class ButtonTextControl : Control
    {
        static ButtonTextControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonTextControl), new FrameworkPropertyMetadata(typeof(ButtonTextControl)));
        }
    }

    public class ButtonsControl : ListBox<ButtonTextControl>
    {
        public static readonly DependencyProperty CommandPathProperty = DependencyProperty.Register("CommandPath", typeof(string), typeof(ButtonsControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ButtonsControl));

        static ButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonsControl), new FrameworkPropertyMetadata(typeof(ButtonsControl)));
        }

        public ButtonsControl()
        {
            this
                .WhenAnyValue(a => a.SelectedValue)
                .Subscribe(a => Output = a);
        }

        #region properties

        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }

        public object Output
        {
            get => GetValue(OutputProperty);
            set => SetValue(OutputProperty, value);
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(ButtonTextControl element, object item)
        {
            if (element.ChildOfType<Button>() is not Button button)
                throw new System.Exception("fdfds ffe33733");

            if (string.IsNullOrEmpty(CommandPath) == false)
                button.Command = (System.Windows.Input.ICommand)item.TryGetValue(CommandPath);

            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(SelectedValuePath) == false)
            {
                BindingOperations.SetBinding(button, FrameworkElement.TagProperty, factory.OneWay(SelectedValuePath));
                button.Click += Button_Click;
            }

            if (string.IsNullOrEmpty(DisplayMemberPath) == false)
            {
                if (button.Content is TextBlock textBlock)
                    BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, factory.OneWay(DisplayMemberPath));
            }
        }

        protected virtual void Button_Click(object sender, RoutedEventArgs e)
        {
            Output = ((FrameworkElement)sender).Tag;
        }
    }
}