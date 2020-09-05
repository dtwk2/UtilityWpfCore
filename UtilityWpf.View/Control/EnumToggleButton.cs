using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

namespace UtilityWpf.View
{
    public class EnumToggleButton : ToggleButton
    {
        private ResourceDictionary resourceDictionary;
        private readonly ISubject<bool> subject = new Subject<bool>();
        private readonly ISubject<Array> enumSubject = new Subject<Array>();

        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register("Enum", typeof(Type), typeof(EnumToggleButton), new PropertyMetadata(typeof(Switch), null, EnumCoerce));
        public static readonly DependencyProperty UseEnumAsContentProperty = DependencyProperty.Register("UseEnumAsContent", typeof(bool?), typeof(EnumToggleButton), new PropertyMetadata(true, UseEnumAsContentChanged));
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EnumToggleButton));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(EnumToggleButton));

        static EnumToggleButton()
        {
            //  DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumToggleButton), new FrameworkPropertyMetadata(typeof(EnumToggleButton)));
            IsCheckedProperty.OverrideMetadata(typeof(EnumToggleButton), new FrameworkPropertyMetadata(false, null, IsCheckedChanged));
        }

        public EnumToggleButton()
        {
            subject.StartWith(true).CombineLatest(enumSubject.StartWith(Array.Empty<Enum>()), (useEnum, enums) => (useEnum, enums))
                .Subscribe(c =>
                {
                    if (!c.useEnum)
                    {
                        resourceDictionary ??= (ResourceDictionary)Application.LoadComponent(new Uri("/UtilityWpf.View;component/Themes/Geometry.xaml", UriKind.Relative));
                        (this as ToggleButton).Content = resourceDictionary["Tick"];
                        (this as ToggleButton).UnCheckedContent = resourceDictionary["Cross_"];
                    }
                    else if (c.enums.Length > 0)
                    {
                        this.Content = c.enums.Cast<Enum>().First();
                        this.UnCheckedContent = c.enums.Cast<Enum>().Last();
                    }
                });

            base.Checked += EnumToggleButton_Checked;
            base.Unchecked += EnumToggleButton_Unchecked;

            void EnumToggleButton_Unchecked(object sender, RoutedEventArgs e)
            {
                RaiseEvent(new EnumEventArgs(EnumToggleButton.SelectedEvent, false));
            }

            void EnumToggleButton_Checked(object sender, RoutedEventArgs e)
            {
                RaiseEvent(new EnumEventArgs(EnumToggleButton.SelectedEvent, true));
            }
            this.Loaded += EnumToggleButton_Loaded;
        }

        private void EnumToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            subject.OnNext(UseEnumAsContent);
        }

        public Type Enum
        {
            get { return (Type)GetValue(EnumProperty); }
            set { SetValue(EnumProperty, Enum); }
        }

        public bool UseEnumAsContent
        {
            get { return (bool)GetValue(UseEnumAsContentProperty); }
            set { SetValue(UseEnumAsContentProperty, value); }
        }

        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        private static object IsCheckedChanged(DependencyObject d, object baseValue)
        {
            var vals = System.Enum.GetValues((Type)(d as EnumToggleButton).Enum);
            if ((bool)baseValue)
                (d as EnumToggleButton).Output = vals.Cast<object>().First();
            else
                (d as EnumToggleButton).Output = vals.Cast<object>().Last();

            return baseValue;
        }

        private static object EnumCoerce(DependencyObject d, object baseValue)
        {
            var vals = System.Enum.GetValues((Type)baseValue);

            if (vals.Length == 2)
            {
                (d as EnumToggleButton).enumSubject.OnNext(vals);
                //(d as ToggleSwitch.ToggleSwitchBase).CheckedContent = vals.Cast<object>().First();
                //(d as ToggleSwitch.ToggleSwitchBase).UncheckedContent = vals.Cast<object>().Last();
            }
            else
            {
                (d as EnumToggleButton).enumSubject.OnNext(new[] { vals.Cast<Enum>().First(), vals.Cast<Enum>().Last() });
            }
            return baseValue;
        }

        private static void UseEnumAsContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as EnumToggleButton).subject.OnNext((bool)e.NewValue);
        }

        // Raise the routed event "selected"
        public class EnumEventArgs : RoutedEventArgs
        {
            public bool IsChecked;

            public EnumEventArgs(RoutedEvent routedEvent, bool isChecked) : base(routedEvent)
            {
                IsChecked = isChecked;
            }
        }

        public enum Switch
        {
            On, Off
        }
    }
}