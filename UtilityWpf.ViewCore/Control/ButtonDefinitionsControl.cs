using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace UtilityWpf.View
{
    public class ButtonDefinitionsControl : ItemsControl
    {
        //public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ItemsSourceChanged));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));

        public static readonly DependencyProperty OutputTypeProperty = DependencyProperty.Register("OutputType", typeof(Type), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));

        public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(IEnumerable), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ButtonDefinitionsControl));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ButtonDefinitionsControl),new PropertyMetadata(Orientation.Horizontal));

        private static void ParametersChange(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ButtonDefinitionsControl).UpdateButtons();

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

        static ButtonDefinitionsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonDefinitionsControl), new FrameworkPropertyMetadata(typeof(ButtonDefinitionsControl)));
        }

        public ButtonDefinitionsControl()
        {
        }

        private void UpdateButtons()
        {

            var items = ButtonDefinitionHelper.GetCommandOutput(Type, OutputType, Parameters)?
                .Select(kvp =>
                new ViewModel.ButtonDefinition
                {

                    Command = new RelayCommand(() => SetOuput(kvp.Value())),
                    Content = kvp.Key
                });

            if (items == null) Console.WriteLine("measurements-service equals null in collectionviewmodel");

            this.Dispatcher.InvokeAsync(() => ItemsSource = items.ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));

            void SetOuput(object a) =>/* _output.OnNext(a)*/
                  this.Dispatcher.InvokeAsync(() => Output = a, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        }
    }
}