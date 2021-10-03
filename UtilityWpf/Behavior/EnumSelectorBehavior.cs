#nullable enable
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
            if (AssociatedObject is ComboBox box)
            {
                box.IsEditable = false;
                AssociatedObject.Width = 200;
                AssociatedObject.Height = 40;
            }

            AssociatedObject.DisplayMemberPath = string.IsNullOrEmpty(AssociatedObject.DisplayMemberPath) ?
                "Description" :
                AssociatedObject.DisplayMemberPath;
            AssociatedObject.SelectedValuePath = "Value";
            AssociatedObject.SelectedIndex = 0;
            AssociatedObject.HorizontalAlignment = HorizontalAlignment.Center;
            AssociatedObject.VerticalAlignment = VerticalAlignment.Center;
            AssociatedObject.HorizontalContentAlignment = HorizontalAlignment.Center;
            AssociatedObject.VerticalContentAlignment = VerticalAlignment.Center;


            this.WhenAnyValue(a => a.SelectedEnum)
            .DistinctUntilChanged()
            .WhereNotNull()
            .CombineLatest(AssociatedObject.WhenAnyValue(a => a.ItemsSource)
                               .WhereNotNull(), (a, b) => (a, b))
            .Subscribe(c =>
            {
                var (@enum, enumerableNullable) = c;
                if (!(enumerableNullable is { } enumerable))
                    return;
                var arr = enumerable.Cast<EnumMember>().ToArray();
                var def = arr.SingleOrDefault(a => a.StringValue == @enum.ToString());
                var index = Array.IndexOf(arr, def);
                AssociatedObject.SetValue(Selector.SelectedIndexProperty, index);
            }).DisposeWith(disposable);

            AssociatedObject.WhenAnyValue(a => a.SelectedValue)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Subscribe(c =>
                {
                    SetValue(SelectedEnumProperty, (Enum?)c);
                }).DisposeWith(disposable);

            itemsSourceSubject.Subscribe(a =>
            {
                AssociatedObject.ItemsSource = a;
            }).DisposeWith(disposable);

            this.WhenAnyValue(a => a.EnumFilterCollection)
                .WhereNotNull()
                .Subscribe(a =>
            {
                var flags = EnumHelper.Filter(a).ToArray();
                var members = EnumMember.EnumerateEnumMembers(a.GetType().GetElementType());
                var aa = from flag in flags
                         join mem in members on flag equals mem.Value
                         select mem;
                var arr = aa.ToArray();
                itemsSourceSubject.OnNext(arr);
            });

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

        ReplaySubject<IEnumerable> itemsSourceSubject = new(1);

        //private static void EnumTypeChanged<T>(DependencyObject d, DependencyPropertyChangedEventArgs e) where T : Enum
        //{
        //    if (!(d is EnumListBehavior { EnumType: { } type } behavior)) return;
        //    Type generic = typeof(EnumMember<>).MakeGenericType(type);
        //    object? instance = Activator.CreateInstance(generic);
        //    MethodInfo? method = instance?.GetType().GetMethod(nameof(EnumMember<T>.EnumerateEnumMembers));
        //    IEnumerable? itemsSource = (IEnumerable?)method?.Invoke(instance, null);
        //    //behavior.AssociatedObject.ItemsSource = itemSource;
        //    if (itemsSource != null)
        //        behavior.itemsSourceSubject.OnNext(itemsSource);
        //    else
        //        throw new ApplicationException("F c44 SDfd");
        //}    

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