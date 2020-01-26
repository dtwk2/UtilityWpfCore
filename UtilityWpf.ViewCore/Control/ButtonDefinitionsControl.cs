using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf;

namespace UtilityWpf.View
{
    public class ButtonDefinitionsControl : ItemsControl
    {
        //public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ItemsSourceChanged));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));

        public static readonly DependencyProperty OutputTypeProperty = DependencyProperty.Register("OutputType", typeof(Type), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));

        public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(IEnumerable), typeof(ButtonDefinitionsControl), new PropertyMetadata(null, ParametersChange));

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ButtonDefinitionsControl));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ButtonDefinitionsControl));

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
            Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/ButtonDefinitionsControl.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Style = resourceDictionary["BDStyle"] as Style;
        }

        private void UpdateButtons()
        {
            Action<object> av = (a) =>/* _output.OnNext(a)*/
                         this.Dispatcher.InvokeAsync(() => Output = a, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));

            var items = ButtonDefinitionHelper.GetCommandOutput(Type, OutputType, Parameters)?
                .Select(meas =>
                new ViewModel.ButtonDefinition
                {
                    Command = new RelayCommand(() => av(meas.Value())),
                    Content = meas.Key
                });

            if (items == null) Console.WriteLine("measurements-service equals null in collectionviewmodel");

            this.Dispatcher.InvokeAsync(() => ItemsSource = items.ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        }
    }
}

//public class ButtonDefinitionsViewModel<T> : ICollectionViewModel<ButtonDefinition>, IOutputViewModel<T>
//{
//    public IObservable<T> Output => _output;

//    public ICollection<ButtonDefinition> Items => _items;

//    private Subject<T> _output;
//    private ICollection<ButtonDefinition> _items = new ObservableCollection<ButtonDefinition>();

//    //public ObservableCollection<ButtonDefinition> Items { get; } = new ObservableCollection<ButtonDefinition>();

//    public ButtonDefinitionsViewModel(IEnumerable<KeyValuePair<String, Func<T>>> kvps, Dispatcher dispatcher, params object[] parameters)
//    {
//        //Output = new ReactiveProperty<T>();
//        //var kvps = ButtonDefinitionHelper.GetCommandOutput<T>(parameters);

//        _output = new Subject<T>();
//        Action<T> av = (a) => _output.OnNext(a);

//        if (kvps != null)
//            dispatcher.Invoke(() =>
//            {
//                foreach (var meas in kvps)
//                {
//                    _items.Add(new ButtonDefinition { Command = new RelayCommand(() => av(meas.Value())), Content = meas.Key });

//                }
//            });
//        else
//            Console.WriteLine("measurements-service equals null in collectionviewmodel");

//    }

//    public ButtonDefinitionsViewModel(IObservable<KeyValuePair<string, Func<T>>> kvps, IScheduler ui)
//    {
//        //Output = new ReactiveProperty<T>();

//        _output = new Subject<T>();
//        Action<T> av = (a) => _output.OnNext(a);

//        if (kvps != null)
//            kvps.Subscribe(meas =>
//                 _items.Add(new ButtonDefinition { Command = new RelayCommand(() => av(meas.Value())), Content = meas.Key }));
//        else
//            Console.WriteLine("measurements-service equals null in collectionviewmodel");

//    }

//}