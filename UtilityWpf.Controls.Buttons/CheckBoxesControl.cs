using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Base;
using UtilityWpf.Utility;

namespace UtilityWpf.Controls.Buttons
{
    public class CheckedRoutedEventArgs : RoutedEventArgs
    {
        public CheckedRoutedEventArgs(RoutedEvent routedEvent, object source, Dictionary<object, bool?> dictionary) : base(routedEvent, source)
        {
            Dictionary = dictionary;
        }

        public Dictionary<object, bool?> Dictionary { get; }
    }

    public delegate void OutputChangedEventHandler(object sender, RoutedEventArgs args);

    public interface IOutput
    {
        event OutputChangedEventHandler OutputChange;

        object Output { get; set; }
    }

    public interface ICheckedPath
    {
        string IsCheckedPath { get; set; }
    }

    public class CheckBoxesControl : ListBox<CheckBox>, ICheckedPath, IOutput
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(CheckBoxesControl));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler), typeof(CheckBoxesControl));

        static CheckBoxesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesControl), new FrameworkPropertyMetadata(typeof(CheckBoxesControl)));
        }

        public CheckBoxesControl()
        {
            this.Loaded += CheckBoxesControl_Loaded;
        }

        private void CheckBoxesControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #region properties

        public event OutputChangedEventHandler OutputChange
        {
            add => AddHandler(OutputChangeEvent, value);
            remove => RemoveHandler(OutputChangeEvent, value);
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        public object Output
        {
            get => GetValue(OutputProperty);
            set => SetValue(OutputProperty, value);
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            CheckBoxesHelper.Bind(element, item, this);

            element.Checked += sdf33;
            element.Unchecked += sdf33;

            sdf33(this, null);

            void sdf33(object sender, RoutedEventArgs? eventArgs)
            {
                var output = CheckBoxesHelper.ToDictionary(this);
                if (output?.Any() == true)
                {
                    this.Output = output;
                    this.RaiseEvent(new CheckedRoutedEventArgs(OutputChangeEvent, sender, output));
                }
            }
        }
    }

    public class CheckBoxesFilteredControl : ListBox<CheckBox>, ICheckedPath
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesFilteredControl), new PropertyMetadata(null));

        static CheckBoxesFilteredControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesFilteredControl), new FrameworkPropertyMetadata(typeof(CheckBoxesFilteredControl)));
        }

        public CheckBoxesFilteredControl()
        {
            this.Loaded += CheckBoxesControl_Loaded;
        }

        private void CheckBoxesControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #region properties

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            CheckBoxesHelper.Bind(element, item, this);

            element.Checked += OnChange;
            element.Unchecked += OnChange;

            OnChange(element, default);

            void OnChange(object sender, RoutedEventArgs? _)
            {
                if (sender is CheckBox { IsChecked: true } checkbox)
                    checkbox.Visibility = Visibility.Visible;
                else if (sender is CheckBox { IsChecked: false } checkbox2)
                    checkbox2.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class CheckBoxesComboControl : ComboBox<CheckBox>, ICheckedPath, IOutput
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesComboControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(CheckBoxesComboControl));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler), typeof(CheckBoxesComboControl));

        static CheckBoxesComboControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesComboControl), new FrameworkPropertyMetadata(typeof(CheckBoxesComboControl)));
        }

        #region properties

        public event OutputChangedEventHandler OutputChange
        {
            add => AddHandler(OutputChangeEvent, value);
            remove => RemoveHandler(OutputChangeEvent, value);
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        public object Output
        {
            get => GetValue(OutputProperty);
            set => SetValue(OutputProperty, value);
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            CheckBoxesHelper.Bind(element, item, this);

            element.Checked += OnChange;
            element.Unchecked += OnChange;
            OnChange(this, null);
        }

        protected virtual void OnChange(object sender, RoutedEventArgs eventArgs)
        {
            var output = CheckBoxesHelper.ToDictionary(this);
            if (output?.Any() == true)
            {
                this.Output = output;
                this.RaiseEvent(new CheckedRoutedEventArgs(OutputChangeEvent, sender, output));
            }
        }
    }

    public class CheckBoxesFilteredComboControl : ComboBox<CheckBox>, ICheckedPath
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesFilteredComboControl), new PropertyMetadata(null));

        static CheckBoxesFilteredComboControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesComboControl), new FrameworkPropertyMetadata(typeof(CheckBoxesComboControl)));
        }

        #region properties

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            CheckBoxesHelper.Bind(element, item, this);

            element.Checked += OnChange;
            element.Unchecked += OnChange;
            OnChange(this, null);
        }

        private void OnChange(object sender, RoutedEventArgs? _)
        {
            if (sender is CheckBox { IsChecked: true } checkbox)
                checkbox.Visibility = Visibility.Visible;
            else if (sender is CheckBox { IsChecked: false } checkbox2)
                checkbox2.Visibility = Visibility.Collapsed;
        }
    }

    public static class CheckBoxesHelper
    {
        public static void Bind(FrameworkElement element, object item, object sender)
        {
            if (sender is not ICheckedPath checkedPath ||
                sender is not System.Windows.Controls.Primitives.Selector selector)
            {
                throw new System.Exception("sdf4 fdgdgp;p;p");
            }

            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(checkedPath.IsCheckedPath) == false)
                element.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, factory.TwoWay(checkedPath.IsCheckedPath));

            if (string.IsNullOrEmpty(selector.SelectedValuePath) == false)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.SelectedValuePath));
            }
            else if (string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.DisplayMemberPath));
            }
            else
            {
                throw new System.Exception($"Expected either {nameof(System.Windows.Controls.Primitives.Selector.SelectedValuePath)} or " +
                    $"{nameof(System.Windows.Controls.Primitives.Selector.DisplayMemberPath)} " +
                    $"not to be null.");
            }
        }

        public static Dictionary<object, bool?>? ToDictionary(object sender)
        {
            if (sender is not System.Windows.Controls.Primitives.Selector selector)
            {
                throw new System.Exception("sdf4 fdgdgp;p;p");
            }

            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false ||
            //    string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            //{
            var items = selector.ItemsOfType<CheckBox>().ToArray();
            var output = items.Where(a => a is { Tag: { } tag }).ToDictionary(a => a.Tag, a => a.IsChecked);
            return output;
            //}
        }
    }

    public class CheckProfile : AutoMapper.Profile
    {
        public CheckProfile()
        {
            CreateMap<CheckedRoutedEventArgs, Dictionary<object, bool?>>()
                .ConvertUsing(a => a.Dictionary);
        }
    }
}