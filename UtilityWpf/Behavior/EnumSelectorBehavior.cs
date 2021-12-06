﻿#nullable enable
using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using static UtilityWpf.Behavior.EnumSelectorBehavior;

namespace UtilityWpf.Behavior
{
    public class EnumSelectorBehavior : Behavior<Selector>
    {
        CompositeDisposable? disposable = null;
        ReplaySubject<IEnumerable> itemsSourceSubject = new(1);

        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumSelectorBehavior), new PropertyMetadata(EnumTypeChanged));
        public static readonly DependencyProperty EnumFilterCollectionProperty = DependencyHelper.Register<IEnumerable>();
        public static readonly DependencyProperty SelectedEnumProperty =
            DependencyProperty.Register("SelectedEnum", typeof(Enum), typeof(EnumSelectorBehavior), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        protected override void OnAttached()
        {
            disposable = new CompositeDisposable();

            FormatAssociatedObject(AssociatedObject);

            this.WhenAnyValue(a => a.SelectedEnum)
                .DistinctUntilChanged()
                .WhereNotNull()
                .CombineLatest(AssociatedObject.WhenAnyValue(a => a.ItemsSource).WhereNotNull())
                .Select(c =>
                {
                    var (@enum, enumerable) = c;
                    var arr = enumerable.Cast<EnumMember>().ToArray();
                    var def = arr.SingleOrDefault(a => a.StringValue == @enum.ToString());
                    return Array.IndexOf(arr, def);
                })
                .Subscribe(index => AssociatedObject.SetValue(Selector.SelectedIndexProperty, index))
                .DisposeWith(disposable);

            AssociatedObject
                .WhenAnyValue(a => a.SelectedValue)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Subscribe(c =>
                {
                    SetValue(SelectedEnumProperty, (Enum?)c);
                }).DisposeWith(disposable);

            itemsSourceSubject
                .Subscribe(a =>
                {
                    AssociatedObject.ItemsSource = a;
                }).DisposeWith(disposable);

            this.WhenAnyValue(a => a.EnumFilterCollection)
                .WhereNotNull()
                .Select(enumerable =>
                {
                    var flags = EnumHelper.Filter(enumerable).ToArray();
                    var members = EnumMember.EnumerateEnumMembers(enumerable.GetType().GetElementType());
                    var aa = from flag in flags
                             join mem in members on flag equals mem.Value
                             select mem;
                    return aa.ToArray();
                })
                .Subscribe(itemsSourceSubject.OnNext)
                .DisposeWith(disposable);

            base.OnAttached();

        }

        protected override void OnDetaching()
        {
            disposable?.Dispose();
            base.OnDetaching();
        }

        #region properties

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        public IEnumerable EnumFilterCollection
        {
            get => (IEnumerable)GetValue(EnumFilterCollectionProperty);
            set => SetValue(EnumFilterCollectionProperty, value);
        }

        public Enum SelectedEnum
        {
            get => (Enum)GetValue(SelectedEnumProperty);
            set => SetValue(SelectedEnumProperty, value);
        }
        #endregion properties

        private static void EnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not EnumSelectorBehavior { EnumType: { } type } behavior) return;

            if (EnumEnumerable(type) is { } enm)
                behavior.itemsSourceSubject.OnNext(enm);
            else
                throw new ApplicationException("F c44 SDfd");
        }

        private static IEnumerable? EnumEnumerable(Type type)
        {
            MethodInfo? method = typeof(EnumMember).GetMethod(nameof(EnumMember.EnumerateEnumMembers));
            IEnumerable? itemsSource = (IEnumerable?)method?.Invoke(null, new[] { type });
            return itemsSource;
        }

        /// <summary>
        /// Simple attribute class for storing String Values
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        public sealed class StringValueAttribute : System.Attribute
        {
            public StringValueAttribute(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
                Value = value;
            }

            public string Value { get; }
        }

        static void FormatAssociatedObject(Selector associatedObject)
        {

            if (associatedObject is ComboBox box)
            {
                box.IsEditable = false;
                if (double.IsNaN(associatedObject.Width))
                    associatedObject.Width = 200;
                if (double.IsNaN(associatedObject.Height))
                    associatedObject.Height = 40;
            }

            associatedObject.DisplayMemberPath = string.IsNullOrEmpty(associatedObject.DisplayMemberPath) ?
                nameof(EnumMember.Description) :
                associatedObject.DisplayMemberPath;


            associatedObject.SelectedValuePath = string.IsNullOrEmpty(associatedObject.DisplayMemberPath) ?
                nameof(EnumMember.Value) :
                associatedObject.SelectedValuePath;
            associatedObject.SelectedValuePath = "Value";
            associatedObject.SelectedIndex = 0;
            associatedObject.HorizontalAlignment = HorizontalAlignment.Center;
            associatedObject.VerticalAlignment = VerticalAlignment.Center;
            associatedObject.HorizontalContentAlignment = HorizontalAlignment.Center;
            associatedObject.VerticalContentAlignment = VerticalAlignment.Center;

        }
    }

    public class EnumMember
    {
        public object Value { get; init; } = default!;

        public string Description { get; init; } = null!;

        public string StringValue { get; init; } = null!;

        /// <exception cref="ArgumentException">T must be of type enumeration.</exception>
        public static IEnumerable<EnumMember> EnumerateEnumMembers(Type type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be of type enumeration.");
            }

            foreach (var memberInfo in type.GetMembers(BindingFlags.Public | BindingFlags.Static))
            {
                var descriptionAttribute = memberInfo.GetCustomAttributes(true).OfType<DescriptionAttribute>().SingleOrDefault();
                var attr = memberInfo.GetCustomAttributes(true).OfType<StringValueAttribute>().SingleOrDefault();

                var enumMember = new EnumMember
                {
                    Value = Enum.Parse(type, memberInfo.Name),
                    Description = attr?.Value ?? descriptionAttribute?.Description ?? memberInfo.Name,
                    StringValue = memberInfo.Name
                };

                yield return enumMember;
            }
        }

    }

    static class EnumHelper
    {
        public static IEnumerable<Enum> Filter(IEnumerable input)
        {
            var elementType = input.GetType().GetElementType();

            foreach (Enum value in Enum.GetValues(elementType))
                if (input.OfType<Enum>().Contains(value))
                    yield return value;
        }

        public static IEnumerable<Enum> GetFlags(object input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (((Enum)input).HasFlag(value))
                    yield return value;
        }
    }
}