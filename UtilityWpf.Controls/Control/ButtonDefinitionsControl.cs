using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    using Command;
    using UtilityWpf.Model;

    public class ButtonDefinitionsControl : ItemsControl
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));
        public static readonly DependencyProperty OutputTypeProperty = DependencyProperty.Register("OutputType", typeof(Type), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));
        public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(IEnumerable), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ButtonDefinitionsControl));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ButtonDefinitionsControl), new PropertyMetadata(Orientation.Horizontal, Changed));


        static ButtonDefinitionsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonDefinitionsControl), new FrameworkPropertyMetadata(typeof(ButtonDefinitionsControl)));
        }

        public ButtonDefinitionsControl()
        {

        }

        public override void OnApplyTemplate()
        {           
            base.OnApplyTemplate();
        }

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public Type OutputType
        {
            get { return (Type)GetValue(OutputTypeProperty); }
            set { SetValue(OutputTypeProperty, value); }
        }

        public IEnumerable Parameters
        {
            get { return (IEnumerable)GetValue(ParametersProperty); }
            set { SetValue(ParametersProperty, value); }
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

        private void UpdateButtons()
        {
            var items = ButtonDefinitionHelper
                .GetCommandOutput(Type, OutputType, Parameters)
                .Select(kvp => new ButtonDefinition(kvp.Key, new RelayCommand(() => SetOuput(kvp.Value()))))
                .ToArray();

            //if (items == null)
            //    throw new Exception("measurements-service equals null in collectionviewmodel");

            this.Dispatcher.InvokeAsync(() => ItemsSource = items);

            void SetOuput(object a) => this.Dispatcher.InvokeAsync(() => Output = a);
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void ParametersChange(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ButtonDefinitionsControl).UpdateButtons();

    }
}