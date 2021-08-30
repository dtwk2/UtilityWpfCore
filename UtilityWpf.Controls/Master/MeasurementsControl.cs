using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Evan.Wpf;
using ReactiveUI;
using UtilityWpf.Behavior;

namespace UtilityWpf.Controls
{
    public class MeasurementsControl : ItemsContentControl
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
            get { return (string)GetValue(DisplayKeyPathProperty); }
            set { SetValue(DisplayKeyPathProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public Type Unit
        {
            get { return (Type)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public IEnumerable EnumFilterCollection
        {
            get { return (IEnumerable)GetValue(EnumFilterCollectionProperty); }
            set { SetValue(EnumFilterCollectionProperty, value); }

        }

        public Enum SelectedUnit
        {
            get { return (Enum)GetValue(SelectedUnitProperty); }
            set { SetValue(SelectedUnitProperty, value); }

        }

        #endregion properties
    }
}
