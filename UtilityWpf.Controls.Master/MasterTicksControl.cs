using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using UtilityWpf.Controls.Dragablz;

namespace UtilityWpf.Controls.Master
{
    public class MasterTicksControl : MasterBindableControl
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(MasterTicksControl), new PropertyMetadata(null));

        static MasterTicksControl()
        {
           // FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterTicksControl), new FrameworkPropertyMetadata(typeof(MasterTicksControl)));
        }

        public MasterTicksControl()
        {
            Position = Dock.Bottom;
            RemoveOrder = RemoveOrder.Selected;
            this.ButtonTypes = ButtonType.Add | ButtonType.Remove;
            this.WhenAnyValue(a => a.ItemsSource)
           .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.IsCheckedPath))
           .Subscribe(a =>
           {
               var (itemsSource, display, isChecked) = a;
               //this.Dispatcher.InvokeAsync(() =>
               //{
                   if ((this.Content ??= new TicksControl()) is TicksControl msn)
                   {
                       msn.ItemsSource = itemsSource;
                       msn.IsCheckedPath = isChecked;
                       msn.DisplayMemberPath = display;
                   }
                   else
                   {
                       throw new ApplicationException("Expected Content to be "+ nameof(TicksControl));
                   }
               //});
           });
        }


        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }
    }
}
