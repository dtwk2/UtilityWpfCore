using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ReactiveUI;
using UnitsNet.Units;
using UtilityWpf.Behavior;
using UtilityWpf.Controls;

namespace UtilityWpf.Demo.SandBox
{
    public class MeasurementsControl : ItemsWrapControl
    {
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(Enum), typeof(MeasurementsControl));

        static MeasurementsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MeasurementsControl), new FrameworkPropertyMetadata(typeof(MeasurementsControl)));
        }
        public static readonly DependencyProperty DisplayKeyPathProperty = NumbersControl.DisplayKeyPathProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(MeasurementsControl));
        public static readonly DependencyProperty PredicateProperty = EnumSelectorBehavior.PredicateProperty.AddOwner(typeof(MeasurementsControl));

        public MeasurementsControl()
        {
            Predicate = new[] { LengthUnit.Centimeter, LengthUnit.AstronomicalUnit };

            this.WhenAnyValue(a => a.Content).OfType<NumbersControl>().CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.DisplayKeyPath))
                .Subscribe(a =>
                {
                    a.First.DisplayMemberPath = a.Second;
                    a.First.DisplayKeyPath = a.Third;
                });
        }

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

        public Enum Unit
        {
            get { return (Enum)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public IEnumerable Predicate
        {
            get { return (IEnumerable)GetValue(PredicateProperty); }
            set { SetValue(PredicateProperty, value); }

        }
    }
}
