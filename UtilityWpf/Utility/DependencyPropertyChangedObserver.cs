using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;

namespace UtilityWpf
{

    public class MetaDataFactory<TControl> where TControl : class
    {
        public static PropertyMetadata Create<T>(Func<TControl, IObserver<T>> observer, T value = default) =>
            new PropertyMetadata(value, PropertyChangedCallbackFactory<TControl>.Create(observer));
    }

    public class PropertyChangedCallbackFactory<TControl> where TControl : class
    {
        public static PropertyChangedCallback Create<T>(Func<TControl, IObserver<T>> observer) =>
    new PropertyChangedCallback((d, e) => new DependencyPropertyChangedObserver<TControl, T>(observer).OnNext(d, e));

    }

    public class DependencyPropertyFactory<TControl> where TControl : class
    {
        public static DependencyProperty Create<T>(string name, Func<TControl, IObserver<T>> observer, T value = default) =>
            DependencyProperty.Register(name, typeof(T), typeof(TControl), MetaDataFactory<TControl>.Create(observer, value));
    }

    public class DependencyPropertyChangedObserver<TR, T> where TR : class
    {
        private readonly Func<TR, IObserver<T>> observer;

        public DependencyPropertyChangedObserver(Func<TR, IObserver<T>> observer) => this.observer = observer;

        public void OnNext(DependencyObject d, DependencyPropertyChangedEventArgs e) => observer(d as TR).OnNext((T)e.NewValue);
    }
}
