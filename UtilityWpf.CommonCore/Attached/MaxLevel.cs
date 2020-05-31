using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class Equality : Control
    {
        public static readonly DependencyProperty Value1Property = DependencyProperty.RegisterAttached("Value1", typeof(object), typeof(Equality), new PropertyMetadata(null, Value1Change));

        public static void SetValue1(DependencyObject target, Boolean value)
        {
            target.SetValue(Value1Property, value);
        }

        public static bool GetValue1(DependencyObject target)
        {
            return (bool)target.GetValue(Value1Property);
        }

        public static readonly DependencyProperty Value2Property = DependencyProperty.RegisterAttached("Value2", typeof(object), typeof(Equality), new PropertyMetadata(null, Value2Change));

        public static void SetValue2(DependencyObject target, Boolean value)
        {
            target.SetValue(Value2Property, value);
        }

        public static bool GetValue2(DependencyObject target)
        {
            return (bool)target.GetValue(Value2Property);
        }

        public static readonly DependencyProperty IsEqualProperty = DependencyProperty.RegisterAttached("IsEqual", typeof(bool), typeof(Equality));

        public static void SetIsEqual(DependencyObject target, Boolean value)
        {
            target.SetValue(IsEqualProperty, value);
        }

        public static bool GetIsEqual(DependencyObject target)
        {
            return (bool)target.GetValue(IsEqualProperty);
        }

        private static void Value1Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Equality).Value1Changes.OnNext(e.NewValue);
        }

        private static void Value2Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Equality).Value1Changes.OnNext(e.NewValue);
        }

        private ISubject<object> Value1Changes = new Subject<object>();
        private ISubject<object> Value2Changes = new Subject<object>();

        public Equality()
        {
            Value1Changes.CombineLatest(Value2Changes, (one, two) => new { one, two }).Subscribe(_ =>
                   {
                       this.SetValue(IsEqualProperty, _.one.Equals(_.two));
                   });
        }
    }
}