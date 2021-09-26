using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityEnum;

namespace UtilityWpf.Controls
{
    public class EnumItemsControl : ItemsControl
    {
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Output), typeof(Enum), typeof(EnumItemsControl), new FrameworkPropertyMetadata(default(Enum)));

        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register("Enum", typeof(Type), typeof(EnumItemsControl), new PropertyMetadata(typeof(Switch)));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(EnumItemsControl), new PropertyMetadata(false));
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;
        public static readonly DependencyProperty OrientationProperty =  DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(EnumItemsControl), new PropertyMetadata(System.Windows.Controls.Orientation.Vertical));

        static EnumItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumItemsControl), new FrameworkPropertyMetadata(typeof(EnumItemsControl)));
        }

        public EnumItemsControl()
        {
            CompositeDisposable? disposable = null;
            this.WhenAnyValue(a => a.Enum)
                .CombineLatest(this.WhenAnyValue(a => a.IsReadOnly))
                .Select(a => BuildFromEnum(a.First, a.Second).ToArray())
                .Subscribe(enums =>
                {
                    disposable?.Dispose();
                    disposable = new();
                    //Enums = enums;
                    ItemsSource = enums;
                    foreach (var item in enums)
                    {
                        item.Command.Subscribe(a =>
                        {
                            Output = a;
                        }).DisposeWith(disposable);
                    }
                });
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }


        public Type Enum
        {
            get { return (Type)GetValue(EnumProperty); }
            set { SetValue(EnumProperty, Enum); }
        }

        public Enum Output
        {
            get { return (Enum)GetValue(OutputProperty); }
            protected set { SetValue(OutputPropertyKey, value); }
        }

        public System.Windows.Controls.Orientation Orientation
        {
            get { return (System.Windows.Controls.Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static IEnumerable<EnumItem> BuildFromEnum(Type t, bool isReadOnly)
        {
            return System.Enum.GetValues(t).Cast<Enum>().Select(a => new EnumItem(a, ReactiveCommand.Create(() => a), isReadOnly));
        }

        public class EnumItem
        {
            public EnumItem(Enum @enum, ReactiveCommand<Unit, Enum> command, bool isReadOnly)
            {
                Enum = @enum;
                Command = command;
                IsReadOnly = isReadOnly;
            }
            public Enum Enum { get; }

            public ReactiveCommand<Unit, Enum> Command { get; }

            public bool IsReadOnly { get; }
        }
    }

    public class EnumItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ReadOnlyTemplate { get; set; }

        public DataTemplate? InteractiveTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not EnumItemsControl.EnumItem { IsReadOnly: { } isReadonly } enumItem)
            {
                throw new ArgumentException($"Unexpected type. Expected {nameof(EnumItemsControl.EnumItem)}");
            }
            return (isReadonly ? ReadOnlyTemplate : InteractiveTemplate) ?? throw new NullReferenceException("fsdeeeee");
        }
    }
}