using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

    public class CheckBoxesControl : ListBox<CheckBox>
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(CheckBoxesControl));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler), typeof(CheckBoxesControl));

        static CheckBoxesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesControl), new FrameworkPropertyMetadata(typeof(CheckBoxesControl)));
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
            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(IsCheckedPath) == false)
                BindingOperations.SetBinding(element, System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, factory.TwoWay(IsCheckedPath));

            if (string.IsNullOrEmpty(SelectedValuePath) == false)
            {
                BindingOperations.SetBinding(element, TagProperty, factory.OneWay(SelectedValuePath));
            }
            else if (string.IsNullOrEmpty(DisplayMemberPath) == false)
            {
                BindingOperations.SetBinding(element, TagProperty, factory.OneWay(DisplayMemberPath));
            }
            else
            {
                throw new System.Exception($"Expected either {nameof(SelectedValuePath)} or {nameof(DisplayMemberPath)} not to be null.");
            }

            element.Checked += Button_Click;
            element.Unchecked += Button_Click;

            Button_Click(null, null);
        }

        protected virtual void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedValuePath) == false ||
                string.IsNullOrEmpty(DisplayMemberPath) == false)
            {
                var items = GetItems().ToArray();
                var output = items.Where(a => a is { Tag: { } tag }).ToDictionary(a => a.Tag, a => a.IsChecked);
                if (output.Any())
                {
                    Output = output;
                    RaiseEvent(new CheckedRoutedEventArgs(OutputChangeEvent, this, output));
                }
            }
        }

        private IEnumerable<CheckBox> GetItems()
        {
            return this.Items.Cast<object>().Select(a => ItemContainerGenerator.ContainerFromItem(a)).Cast<CheckBox>();
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