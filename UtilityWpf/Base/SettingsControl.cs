using ReactiveUI;
using System;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Behavior;

namespace UtilityWpf.Base
{
    public class SettingsControl : Control
    {
        public delegate void CheckedRoutedEventHandler(object sender, CheckedEventArgs e);

        public enum CheckedType
        {
            DataContext,
            Dimensions
        }

        public class CheckedEventArgs : RoutedEventArgs
        {
            public CheckedEventArgs(CheckedType checkedType, bool isChecked, RoutedEvent routedEvent, object source):base(routedEvent, source)
            {
                CheckedType = checkedType;
                IsChecked = isChecked;
            }

            public CheckedType CheckedType { get; }
            public bool IsChecked { get; }
        }

        public static readonly DependencyProperty SelectedDockProperty = DependencyProperty.Register("SelectedDock", typeof(Dock?), typeof(SettingsControl), new PropertyMetadata(null, Change /*m,coerceValueCallback: Callback*/));
        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(CheckedRoutedEventHandler), typeof(SettingsControl));
        private MenuItem? dimensionsMenuItem;
        private MenuItem? dataContextMenuItem;

        static SettingsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsControl), new FrameworkPropertyMetadata(typeof(SettingsControl)));
        }

        public event CheckedRoutedEventHandler Checked
        {
            add => AddHandler(CheckedEvent, value);         
            remove => RemoveHandler(CheckedEvent, value);
        }

        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            var behavior = this.GetTemplateChild("MenuItemButtonGroupBehavior") as MenuItemButtonGroupBehavior;
            behavior.WhenAnyValue(a => a.SelectedItem)
                .Subscribe(a =>
                {
                    if (a is null)
                        return;
                    if (a is EnumMember { Value: null })
                    {
                        SelectedDock = null;
                        return;
                    }
                    if (a is EnumMember { Value: Dock value })
                    {
                        SelectedDock = value;
                        return;
                    }

                    throw new Exception("fd   99f");
                });

            //<MenuItem x:Name="HeightMenuItem"  Header="Height" IsCheckable="True"></MenuItem>
            //                  <MenuItem x:Name="DataContextMenuItem"  Header="DataContext" IsCheckable="True"></MenuItem>

            dimensionsMenuItem = this.GetTemplateChild("DimensionsMenuItem") as MenuItem;
            dimensionsMenuItem.Checked += DimensionsMenuItem_Checked;
            dimensionsMenuItem.Unchecked += DimensionsMenuItem_Checked;

            dataContextMenuItem = this.GetTemplateChild("DataContextMenuItem") as MenuItem;
            dataContextMenuItem.Checked += DataContextMenuItem_Checked;
            dataContextMenuItem.Unchecked += DataContextMenuItem_Checked;

            base.OnApplyTemplate();
        }

        private void DataContextMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(CheckedType.DataContext, dataContextMenuItem.IsChecked, CheckedEvent, this));
        }

        private void DimensionsMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(CheckedType.Dimensions, dimensionsMenuItem.IsChecked, CheckedEvent, this));
        }

        //private static object Callback(DependencyObject d, object baseValue)
        //{
        //    if (baseValue is EnumMember { Value: var value } member)
        //    {
        //        return value;
        //    }
        //    throw new Exception("DGF£FGVV vv446");
        //}

        public Dock? SelectedDock
        {
            get { return (Dock?)GetValue(SelectedDockProperty); }
            set { SetValue(SelectedDockProperty, value); }
        }
    }
}
