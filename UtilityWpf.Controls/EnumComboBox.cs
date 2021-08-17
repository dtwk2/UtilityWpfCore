#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReactiveUI;

namespace UtilityWpf.Controls
{
    public class EnumComboBox : ComboBox
    {
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumComboBox), new PropertyMetadata(EnumTypeChanged<Visibility>));



        public Enum SelectedEnumValue
        {
            get => (Enum)GetValue(SelectedEnumValueProperty);
            set => SetValue(SelectedEnumValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedEnumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedEnumValueProperty =
            DependencyProperty.Register("SelectedEnumValue", typeof(Enum), typeof(EnumComboBox), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        static EnumComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumComboBox), new FrameworkPropertyMetadata(typeof(EnumComboBox)));
        }

        public EnumComboBox()
        {
            IsEditable = false;
            DisplayMemberPath = "Description";
            SelectedValuePath = "Value";

            this.WhenAnyValue(a => a.SelectedEnumValue)
                .DistinctUntilChanged()
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.ItemsSource)
                                   .WhereNotNull(), (a, b) => (a, b))
                .Subscribe(c =>
                {
                    var (@enum, enumerableNullable) = c;
                    if (!(enumerableNullable is { } enumerable))
                        return;
                    var arr = enumerable.Cast<EnumMemberBase>().ToArray();
                    var def = arr.SingleOrDefault(a => a.StringValue == @enum.ToString());
                    var index = Array.IndexOf(arr, def);
                    SetValue(SelectedIndexProperty, index);
                });

            this.WhenAnyValue(a => a.SelectedValue)
                .WhereNotNull()
                .DistinctUntilChanged()

                .Subscribe(c =>
                {
                    SetValue(SelectedEnumValueProperty, (Enum?)c);
                });
        }

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        private static void EnumTypeChanged<T>(DependencyObject d, DependencyPropertyChangedEventArgs e) where T : Enum
        {
            if (!(d is EnumComboBox { EnumType: { } type } comboBox)) return;
            Type generic = typeof(EnumMember<>).MakeGenericType(type);
            object? instance = Activator.CreateInstance(generic);
            MethodInfo? method = instance?.GetType().GetMethod(nameof(EnumMember<T>.EnumerateEnumMembers));
            IEnumerable? itemSource = (IEnumerable?)method?.Invoke(instance, null);
            comboBox.ItemsSource = itemSource;
        }

        public class EnumMember<T> : EnumMemberBase
            where T : Enum
        {

            public T Value { get; set; } = default!;

            /// <exception cref="ArgumentException">T must be of type enumeration.</exception>
            public IEnumerable<EnumMember<T>> EnumerateEnumMembers()
            {
                Type type = typeof(T);

                if (!type.IsEnum)
                {
                    throw new ArgumentException("T must be of type enumeration.");
                }

                foreach (var memberInfo in type.GetMembers(BindingFlags.Public | BindingFlags.Static))
                {
                    var descriptionAttribute = memberInfo.GetCustomAttributes(true).OfType<DescriptionAttribute>().SingleOrDefault();
                    var attr = memberInfo.GetCustomAttributes(true).OfType<StringValueAttribute>().SingleOrDefault();

                    var enumMember = new EnumMember<T>
                    {
                        Value = (T)Enum.Parse(type, memberInfo.Name),
                        Description = attr?.Value ?? descriptionAttribute?.Description ?? memberInfo.Name,
                        StringValue = memberInfo.Name
                    };

                    yield return enumMember;
                }
            }
        }

        public class EnumMemberBase
        {
            public string Description { get; set; } = null!;


            public string StringValue { get; set; } = null!;
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
}