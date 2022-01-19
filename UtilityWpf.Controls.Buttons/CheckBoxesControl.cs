using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CheckBoxesControl), new PropertyMetadata(Orientation.Vertical, Changed));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler), typeof(CheckBoxesControl));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        static CheckBoxesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesControl), new FrameworkPropertyMetadata(typeof(CheckBoxesControl)));
        }

        #region properties

        public event OutputChangedEventHandler OutputChange
        {
            add { AddHandler(OutputChangeEvent, value); }
            remove { RemoveHandler(OutputChangeEvent, value); }
        }

        public string IsCheckedPath
        {
            get { return (string)GetValue(IsCheckedPathProperty); }
            set { SetValue(IsCheckedPathProperty, value); }
        }

        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            if (string.IsNullOrEmpty(IsCheckedPath) == false)
                BindingOperations.SetBinding(element, CheckBox.IsCheckedProperty, CreateBinding(IsCheckedPath, BindingMode.TwoWay));

            if (string.IsNullOrEmpty(SelectedValuePath) == false)
            {
                BindingOperations.SetBinding(element, TagProperty, CreateBinding(SelectedValuePath));
            }
            else if (string.IsNullOrEmpty(DisplayMemberPath) == false)
            {
                BindingOperations.SetBinding(element, TagProperty, CreateBinding(DisplayMemberPath));
            }

            element.Checked += Button_Click;
            element.Unchecked += Button_Click;

            Binding CreateBinding(string path, BindingMode bindingMode = BindingMode.OneWay)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(path),
                    Mode = bindingMode,
                };
            }
            Button_Click(null, null);
        }

        protected virtual void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedValuePath) == false ||
                string.IsNullOrEmpty(DisplayMemberPath) == false)
            {
                var items = GetItems().ToArray();
                var output = items.Where(a => a is CheckBox { Tag: { } tag }).ToDictionary(a => a.Tag, a => a.IsChecked);
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