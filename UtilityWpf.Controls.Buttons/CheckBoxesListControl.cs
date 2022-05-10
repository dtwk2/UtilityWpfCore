using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Abstract;
using UtilityWpf.Base;
using UtilityWpf.Controls.Buttons.Infrastructure;

namespace UtilityWpf.Controls.Buttons
{
    public class CheckBoxesListControl : ListBox<CheckBox>, IIsCheckedPath, IOutput<CheckedRoutedEventArgs>
    {
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Output), typeof(object), typeof(CheckBoxesListControl), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;
        private DifferenceHelper differenceHelper;
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler<CheckedRoutedEventArgs>), typeof(CheckBoxesListControl));

        static CheckBoxesListControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesListControl), new FrameworkPropertyMetadata(typeof(CheckBoxesListControl)));
        }

        public CheckBoxesListControl()
        {
            this.Loaded += OnChange;
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
}