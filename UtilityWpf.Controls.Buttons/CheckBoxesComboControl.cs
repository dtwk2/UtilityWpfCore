using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Utility.WPF.Helper;
using UtilityWpf.Abstract;
using UtilityWpf.Base;
using UtilityWpf.Controls.Buttons.Infrastructure;

namespace UtilityWpf.Controls.Buttons
{
    public class CheckBoxesComboControl : ComboBox<CheckBox>, IIsCheckedPath, IOutput<CheckedRoutedEventArgs>, IIsSelectedPath
    {
        private DifferenceHelper differenceHelper;
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Output), typeof(object), typeof(CheckBoxesComboControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesComboControl), new PropertyMetadata(null));
        //public static readonly DependencyProperty OutputProperty = DependencyProperty.RegisterReadOnly("Output", typeof(object), typeof(CheckBoxesComboControl));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler<CheckedRoutedEventArgs>), typeof(CheckBoxesComboControl));
        public static readonly DependencyProperty IsSelectedPathProperty = DependencyProperty.Register("IsSelectedPath", typeof(string), typeof(CheckBoxesComboControl));
        public static readonly DependencyProperty IsDisabledShownProperty = DependencyProperty.Register("IsDisabledShown", typeof(bool), typeof(CheckBoxesComboControl));
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        static CheckBoxesComboControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesComboControl), new FrameworkPropertyMetadata(typeof(CheckBoxesComboControl)));
        }

        public CheckBoxesComboControl()
        {
            this.Loaded += OnChange;
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.CloseButtonClick), true);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
             OnChange(sender, e);
        }

        #region properties

        public event OutputChangedEventHandler<CheckedRoutedEventArgs> OutputChange
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
            protected set => SetValue(OutputPropertyKey, value);
        }

        public string IsSelectedPath
        {
            get { return (string)GetValue(IsSelectedPathProperty); }
            set { SetValue(IsSelectedPathProperty, value); }
        }

        public bool IsDisabledShown
        {
            get { return (bool)GetValue(IsDisabledShownProperty); }
            set { SetValue(IsDisabledShownProperty, value); }
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            CheckBoxesHelper.Bind(element, item, this);
            element.Checked += OnChange;
            element.Unchecked += OnChange;
        }

        protected virtual void OnChange(object sender, RoutedEventArgs eventArgs)
        {
            var dictionary = (differenceHelper ??= new DifferenceHelper(this)).Get;
            Output = dictionary;
            this.RaiseEvent(new CheckedRoutedEventArgs(OutputChangeEvent, this, dictionary.ToArray()));
        }
    }



    /// <summary>
    /// Goes inside CheckBoxesComboControl
    /// </summary>
    public class CheckBoxesFilteredControl : ListBox, IIsCheckedPath, IIsSelectedPath
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesFilteredControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsDisabledShownProperty = DependencyProperty.Register("IsDisabledShown", typeof(bool), typeof(CheckBoxesFilteredControl));
        public static readonly DependencyProperty IsSelectedPathProperty = CheckBoxesComboControl.IsSelectedPathProperty.AddOwner(typeof(CheckBoxesFilteredControl));

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

        public bool IsDisabledShown
        {
            get { return (bool)GetValue(IsDisabledShownProperty); }
            set { SetValue(IsDisabledShownProperty, value); }
        }

        public string IsSelectedPath
        {
            get { return (string)GetValue(IsSelectedPathProperty); }
            set { SetValue(IsSelectedPathProperty, value); }
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(DependencyObject dependencyObject, object item)
        {
            if (dependencyObject is not FrameworkElement element)
            {
                throw new System.Exception("re4gfdg");
            }

            Bind(element, item, this);

            element.IsEnabledChanged += OnChange;

            OnChange(element, default);

            void OnChange(object sender, DependencyPropertyChangedEventArgs _)
            {
                switch (sender)
                {
                    case FrameworkElement { IsEnabled: true } checkbox:
                        checkbox.Visibility = Visibility.Visible;
                        break;

                    default:
                        if (IsDisabledShown == false && sender is FrameworkElement { IsEnabled: false } checkbox2)
                            checkbox2.Visibility = Visibility.Collapsed;
                        break;
                }
            }

            base.PrepareContainerForItemOverride(dependencyObject, item);
        }

        public void Bind(FrameworkElement element, object item, object sender)
        {
            if (sender is not IIsCheckedPath checkedPath ||
                sender is not IIsSelectedPath selectedPath ||
                sender is not System.Windows.Controls.Primitives.Selector selector)
            {
                throw new System.Exception("sdf4 fdgdgp;p;p");
            }

            element.AddHandler(Button.ClickEvent, new System.Windows.RoutedEventHandler(this.CloseButtonClick), true);

            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(checkedPath.IsCheckedPath) == false)
                element.SetBinding(FrameworkElement.IsEnabledProperty, factory.TwoWay(checkedPath.IsCheckedPath));

            if (string.IsNullOrEmpty(selectedPath.IsSelectedPath) == false)
                element.SetBinding(ListBoxItem.IsSelectedProperty, factory.TwoWay(selectedPath.IsSelectedPath));

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
                throw new System.Exception($"Expected either {nameof(SelectedValuePath)} or " +
                    $"{nameof(DisplayMemberPath)} " +
                    $"not to be null.");
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.IsEnabled = false;
            }
        }
    }
}