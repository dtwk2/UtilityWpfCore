using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using ToggleSwitch;

namespace UtilityWpf.View
{
    public class EnumToggleButton : HorizontalToggleSwitch
    {
        public object Enum
        {
            get { return (object)GetValue(EnumProperty); }
            set { SetValue(EnumProperty, value); }
        }

        static EnumToggleButton()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumToggleButton), new FrameworkPropertyMetadata(typeof(EnumToggleButton)));
            IsCheckedProperty.OverrideMetadata(typeof(EnumToggleButton), new FrameworkPropertyMetadata(false, null, IsCheckedChanged));
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

        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register("Enum", typeof(object), typeof(EnumToggleButton), new PropertyMetadata(typeof(Switch), null, EnumCoerce));

        private static object EnumCoerce(DependencyObject d, object baseValue)
        {
            var vals = System.Enum.GetValues((Type)baseValue);

            if (vals.Length == 2)
            {
                (d as EnumToggleButton).enumSubject.OnNext(vals);
                //(d as ToggleSwitch.ToggleSwitchBase).CheckedContent = vals.Cast<object>().First();
                //(d as ToggleSwitch.ToggleSwitchBase).UncheckedContent = vals.Cast<object>().Last();
            }
            return baseValue;
        }

        public bool UseEnumAsContent
        {
            get { return (bool)GetValue(UseEnumAsContentProperty); }
            set { SetValue(UseEnumAsContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseEnumAsContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseEnumAsContentProperty =
            DependencyProperty.Register("UseEnumAsContent", typeof(bool), typeof(EnumToggleButton), new PropertyMetadata(true, UseEnumAsContentChanged));

        private ISubject<bool> subject = new Subject<bool>();
        private ISubject<Array> enumSubject = new Subject<Array>();

        private static void UseEnumAsContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as EnumToggleButton).subject.OnNext((bool)e.NewValue);
        }

        public EnumToggleButton()
        {
            Uri resourceLocater = null;
            ResourceDictionary resourceDictionary = null;
            subject.StartWith(true).CombineLatest(enumSubject.StartWith(Array.Empty<Enum>()), (a, b) => new { a, b }).Subscribe(_ =>
                {
                    if (!_.a)
                    {
                        resourceLocater = resourceLocater ?? new Uri("/UtilityWpf.ViewCore;component/Themes/EnumToggleButton.xaml", System.UriKind.Relative);
                        resourceDictionary = resourceDictionary ?? (ResourceDictionary)Application.LoadComponent(resourceLocater);
                    //Style = resourceDictionary["EnumToggleButtonStyle"] as Style;
                    (this as ToggleSwitch.ToggleSwitchBase).CheckedContent = resourceDictionary["Tick"];
                        (this as ToggleSwitch.ToggleSwitchBase).UncheckedContent = resourceDictionary["Cross"];
                    }
                    else if (_.b.Length > 0)
                    {
                        (this as ToggleSwitch.ToggleSwitchBase).CheckedContent = _.b.Cast<Enum>().First();
                        (this as ToggleSwitch.ToggleSwitchBase).UncheckedContent = _.b.Cast<Enum>().Last();
                    }
                });

            base.Checked += EnumToggleButton_Checked;
            base.Unchecked += EnumToggleButton_Unchecked;
        }

        private void EnumToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new EnumEventArgs(EnumToggleButton.SelectedEvent, false));
        }

        private void EnumToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new EnumEventArgs(EnumToggleButton.SelectedEvent, true));
        }

        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(EnumToggleButton));

        public enum Switch
        {
            On, Off
        }

        // Register the routed event
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EnumToggleButton));

        // .NET wrapper
        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
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
    }
}