﻿using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using UtilityWpf.Controls.Dragablz;
using UtilityWpf.Controls.Master;

namespace UtilityWpf.Controls.Hybrid
{
    public class MasterTicksControl : MasterBindableControl
    {
        public static readonly DependencyProperty CommandPathProperty =
      DependencyProperty.Register("CommandPath", typeof(string), typeof(ButtonsControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(MasterTicksControl), new PropertyMetadata(null));

        static MasterTicksControl()
        {
            // FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterTicksControl), new FrameworkPropertyMetadata(typeof(MasterTicksControl)));
        }

        public MasterTicksControl()
        {
            Position = Dock.Bottom;
            RemoveOrder = RemoveOrder.Selected;
            ButtonTypes = ButtonType.Add | ButtonType.Remove;
            this.WhenAnyValue(a => a.ItemsSource)
           .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.IsCheckedPath), this.WhenAnyValue(a => a.CommandPath))
           .Subscribe(a =>
           {
               var (itemsSource, display, isChecked, commandPath) = a;
               //this.Dispatcher.InvokeAsync(() =>
               //{
               if ((Content ??= new TicksControl()) is TicksControl msn)
               {
                   msn.ItemsSource = itemsSource;
                   msn.IsCheckedPath = isChecked;
                   msn.CommandPath = commandPath;
                   msn.DisplayMemberPath = display;
               }
               else
               {
                   throw new ApplicationException("Expected Content to be " + nameof(TicksControl));
               }
               //});
           });
        }


        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        public string CommandPath
        {
            get => (string)GetValue(CommandPathProperty);
            set => SetValue(CommandPathProperty, value);
        }
    }
}
