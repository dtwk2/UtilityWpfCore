﻿using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Controls.Dragablz;
using UtilityWpf.Controls.Master;

namespace UtilityWpf.Controls.Hybrid
{
    public class MasterListControl
        : MasterBindableControl
    {
        public static readonly DependencyProperty CommandPathProperty = DependencyProperty.Register("CommandPath", typeof(string), typeof(MasterListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(MasterListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsRefreshablePathProperty = DependencyProperty.Register("IsRefreshablePath", typeof(string), typeof(MasterListControl), new PropertyMetadata(null));


        static MasterListControl()
        {
            // FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterTicksControl), new FrameworkPropertyMetadata(typeof(MasterTicksControl)));
        }

        public MasterListControl()
        {
            Position = Dock.Bottom;
            RemoveOrder = RemoveOrder.Selected;
            ButtonTypes = ButtonType.Add | ButtonType.Remove;
            this.WhenAnyValue(a => a.ItemsSource)
           .CombineLatest(
           this.WhenAnyValue(a => a.DisplayMemberPath),
           this.WhenAnyValue(a => a.IsCheckedPath),
           this.WhenAnyValue(a => a.CommandPath),
           this.WhenAnyValue(a => a.IsRefreshablePath)
           )
           .Subscribe(a =>
           {
               var (itemsSource, display, isChecked, commandPath, isRefreshable) = a;
               //this.Dispatcher.InvokeAsync(() =>
               //{
               if ((Content ??= new ListControl()) is ListControl msn)
               {
                   msn.ItemsSource = itemsSource;
                   msn.IsCheckedPath = isChecked;
                   msn.CommandPath = commandPath;
                   msn.DisplayMemberPath = display;
                   msn.IsRefreshablePath = isRefreshable;
               }
               else
               {
                   throw new ApplicationException("Expected Content to be " + nameof(ListControl));
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

        public string IsRefreshablePath
        {
            get { return (string)GetValue(IsRefreshablePathProperty); }
            set { SetValue(IsRefreshablePathProperty, value); }
        }

    }
}
