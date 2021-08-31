using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using UtilityHelper;
using UtilityWpf.Mixins;

namespace UtilityWpf
{

    public class DependencyPropertyFactory<TControl> where TControl : DependencyObject
    {

        public static DependencyProperty Register<TProperty>(string? name = null, TProperty? initialValue = default, CoerceValueCallback? callBack = null)
        {
            return DependencyProperty.Register(name ??= GetName(typeof(TProperty), name), typeof(TProperty), typeof(TControl), GetPropertyMetadata(name, initialValue, callBack));

            static PropertyMetadata GetPropertyMetadata(string name, TProperty? initialValue, CoerceValueCallback? callBack)
            {
                return typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
                      MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer<TProperty>(name) ?? throw new Exception("XcxcX"), initialValue, callBack) :
                      new PropertyMetadata(initialValue, (a, b) => { }, coerceValueCallback: callBack);
            }
        }
        public static DependencyProperty Register(string name, CoerceValueCallback? callBack = null)
        {
            return DependencyProperty.Register(name, GetProperty(name).PropertyType, typeof(TControl), GetPropertyMetadata(name, callBack));

            static PropertyMetadata GetPropertyMetadata(string name, CoerceValueCallback? callBack)
            {
                return typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
                      MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer(name) ?? throw new Exception("XcxcX"), callBack) :
                      new PropertyMetadata(default, (a, b) => { }, coerceValueCallback: callBack);
            }
        }

        public static DependencyProperty Register(string name, Type propertyType, CoerceValueCallback? callBack = null)
        {
            return DependencyProperty.Register(name, propertyType, typeof(TControl), GetPropertyMetadata(name, callBack));

            static PropertyMetadata GetPropertyMetadata(string name, CoerceValueCallback? callBack)
            {
                return typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
                      MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer(name) ?? throw new Exception("XcxcX"), callBack) :
                      new PropertyMetadata(default, (a, b) => { }, coerceValueCallback: callBack);
            }
        }

        static string GetName(Type propertyType, string? name = null)
        {
            return name ?? GetProperty(propertyType).Name;
        }

        static PropertyInfo GetProperty(Type propertyType)
        {
            var props = typeof(TControl).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                 .Where(a => a != null)
                                                 .Where(p => p.PropertyType == propertyType && p.DeclaringType?.FullName?.StartsWith("System") == false)
                                                 .ToArray();

            if (props.Length > 1)
            {
                throw new Exception("Number of matching properties exceeds one. Use name to make search more specific.");
            }
            if (props.Length == 0)
            {
                throw new Exception($"Number of matching properties is 0. Specify a name for property {propertyType.Name}");
            }
            return props.Single();
        }

        static PropertyInfo GetProperty(string name)
        {
            var props = typeof(TControl).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        .Where(a => a != null)
                                        .Where(p => p.Name == name && p.DeclaringType?.FullName?.StartsWith("System") == false)
                                        .ToArray();

            if (props.Length > 1)
            {
                throw new Exception("Number of matching properties exceeds one. Use name to make search more specific.");
            }
            if (props.Length == 0)
            {
                throw new Exception($"Number of matching properties is 0. Specify a name for property {name}");
            }
            return props.Single();
        }

        public static DependencyProperty Register<TProperty>(string name, Func<TControl, IObserver<TProperty>> observer, TProperty? initialValue = default) =>
            DependencyProperty.Register(name, typeof(TProperty), typeof(TControl), MetaDataFactory<TControl>.Create(observer, initialValue));
    }

    class MetaDataFactory<TControl> where TControl : DependencyObject
    {
        public static PropertyMetadata Create<TProperty>(string? name = null, TProperty? initialValue = default, CoerceValueCallback? callBack = null) =>
         typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
            MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer<TProperty>(name) ?? throw new Exception("XX"), initialValue, callBack) :
           new PropertyMetadata(initialValue);

        public static PropertyMetadata Create<TProperty>(Func<TControl, IObserver<TProperty>> observer, TProperty? value = default, CoerceValueCallback? callBack = null) =>
            new(value, PropertyChangedCallbackFactory.Create(observer!, value), callBack);

        class PropertyChangedCallbackFactory
        {
            public static PropertyChangedCallback Create<T>(Func<TControl, IObserver<T>> observer, T initialValue) =>
new((d, e) => new DependencyPropertyChangedObserver<TControl, T>(observer, initialValue).OnNext(d, e));
        }
    }


    class DependencyPropertyChangedObserver<TControl, T> where TControl : DependencyObject
    {
        private readonly Func<TControl, IObserver<T>> observer;

        public DependencyPropertyChangedObserver(Func<TControl, IObserver<T>> observer, T intialValue)
        {
            this.observer = observer;
        }

        public void OnNext(DependencyObject d, DependencyPropertyChangedEventArgs e) => observer(d as TControl).OnNext((T)e.NewValue);
    }



    public static class DependencyObjectHelper
    {
        public static void Convert<T, TS, TR>(this DependencyObject a, DependencyPropertyChangedEventArgs e, Func<T, TS> map)
            where T : DependencyObject where TS : IObserver<TR>
            => map(a as T).OnNext((TR)e.NewValue);
    }
}
