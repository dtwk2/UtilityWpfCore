using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;

namespace UtilityWpf
{



    public class DependencyPropertyFactory<TControl> where TControl : class
    {
        public static DependencyProperty Register(string name) => DependencyProperty.Register(name, typeof(TControl).GetProperty(name).PropertyType, typeof(TControl));

        public static DependencyProperty Register<TProperty>(string? name = null, TProperty? initialValue = default) =>
         DependencyProperty.Register(name ?? typeof(TProperty).Name, typeof(TProperty), typeof(TControl), new PropertyMetadata(initialValue));

        public static DependencyProperty Register<TProperty>(string name, Func<TControl, IObserver<TProperty>> observer, TProperty? initialValue = default) =>
            DependencyProperty.Register(name, typeof(TProperty), typeof(TControl), MetaDataFactory.Create(observer, initialValue));

        public static DependencyProperty Register<TProperty>(Func<TControl, IObserver<TProperty>> observer, TProperty? initialValue = default) =>
          DependencyProperty.Register(typeof(TProperty).Name, typeof(TProperty), typeof(TControl), MetaDataFactory.Create(observer, initialValue));

        class MetaDataFactory
        {
            public static PropertyMetadata Create<T>(Func<TControl, IObserver<T>> observer, T value = default) =>
                new PropertyMetadata(value, PropertyChangedCallbackFactory.Create(observer));

            class PropertyChangedCallbackFactory
            {
                public static PropertyChangedCallback Create<T>(Func<TControl, IObserver<T>> observer) =>
            new PropertyChangedCallback((d, e) => new DependencyPropertyChangedObserver<TControl, T>(observer).OnNext(d, e));

                class DependencyPropertyChangedObserver<TR, T> where TR : class
                {
                    private readonly Func<TR, IObserver<T>> observer;

                    public DependencyPropertyChangedObserver(Func<TR, IObserver<T>> observer) => this.observer = observer;

                    public void OnNext(DependencyObject d, DependencyPropertyChangedEventArgs e) => observer(d as TR).OnNext((T)e.NewValue);
                }
            }
        }
    }



    public static class DependencyObjectHelper
    {
        public static void Convert<T, TS, TR>(this DependencyObject a, DependencyPropertyChangedEventArgs e, Func<T, TS> map)
            where T : DependencyObject where TS : IObserver<TR>
            => map(a as T).OnNext((TR)e.NewValue);
    }
}
