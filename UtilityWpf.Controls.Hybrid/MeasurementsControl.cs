﻿using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Base;
using UtilityWpf.Behavior;

namespace UtilityWpf.Controls.Hybrid
{
    public class MeasurementsControl : SelectorAndContentControl
    {
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(Type), typeof(MeasurementsControl));
        public static readonly DependencyProperty DisplayKeyPathProperty = NumbersControl.DisplayKeyPathProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty EnumFilterCollectionProperty = EnumSelectorBehavior.EnumFilterCollectionProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty SelectedUnitProperty = DependencyHelper.Register<Enum>();

        static MeasurementsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MeasurementsControl), new FrameworkPropertyMetadata(typeof(MeasurementsControl)));
        }

        public MeasurementsControl()
        {
            this.WhenAnyValue(a => a.DisplayMemberPath).CombineLatest(this.WhenAnyValue(a => a.DisplayKeyPath))
                 .Subscribe(a =>
                 {
                     var xx = new NumbersControl
                     {
                         DisplayMemberPath = a.First,
                         DisplayKeyPath = a.Second
                     };
                     this.Content = xx;
                 });
        }

        #region properties

        public string DisplayKeyPath
        {
            get => (string)GetValue(DisplayKeyPathProperty);
            set => SetValue(DisplayKeyPathProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public Type Unit
        {
            get => (Type)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public IEnumerable EnumFilterCollection
        {
            get => (IEnumerable)GetValue(EnumFilterCollectionProperty);
            set => SetValue(EnumFilterCollectionProperty, value);
        }

        public Enum SelectedUnit
        {
            get => (Enum)GetValue(SelectedUnitProperty);
            set => SetValue(SelectedUnitProperty, value);
        }

        #endregion properties
    }
}