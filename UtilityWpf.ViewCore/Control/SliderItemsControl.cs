using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper.NonGeneric;

namespace UtilityWpf.View
{
    public class SliderItemsControl : Controlx
    {
        //Dictionary<string, Subject<object>> dict = typeof(SliderItemsControl).GetDependencyProperties().ToDictionary(_ => _.Name.Substring(0, _.Name.Length - 8), _ => new Subject<object>());
        private ItemsControl ItemsControl;

        private StackPanel KeyValuePanel;
        private TextBlock at;
        private TextBlock bt;
        private readonly Subject<object> subject = new Subject<object>();

        static SliderItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderItemsControl), new FrameworkPropertyMetadata(typeof(SliderItemsControl)));
        }

        public ObservableCollection<KeyRange> KeyRangeCollection { get; } = new ObservableCollection<KeyRange>();

        public override void OnApplyTemplate()
        {
            ItemsControl = this.GetTemplateChild("ItemsControl") as ItemsControl;
            ItemsControl.ItemsSource = KeyRangeCollection;
            subject.OnNext(ItemsControl);
            KeyValuePanel = this.GetTemplateChild("KeyValuePanel") as StackPanel;
            at = KeyValuePanel.Children.First() as TextBlock;
            bt = KeyValuePanel.Children.OfType<TextBlock>().Last() as TextBlock;
            ValueChanged += SliderItemsControl_ValueChanged;
        }

        private void SliderItemsControl_ValueChanged(object sender, RoutedEventArgs e)
        {
            KeyValuePairRoutedEventArgs keyValuePairRoutedEventArgs = e as KeyValuePairRoutedEventArgs;
            this.Dispatcher.InvokeAsync(() =>
            {
                at.Text = keyValuePairRoutedEventArgs.KeyValuePair.Key;
                bt.Text = keyValuePairRoutedEventArgs.KeyValuePair.Value.ToString("00.###");
            });
        }

        public bool ShowKeyValuePanel
        {
            get { return (bool)GetValue(ShowKeyValuePanelProperty); }
            set { SetValue(ShowKeyValuePanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowKeyValuePanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowKeyValuePanelProperty =
            DependencyProperty.Register("ShowKeyValuePanel", typeof(bool), typeof(SliderItemsControl), new PropertyMetadata(true, Changed));

        public object KeyValuePair
        {
            get { return (object)GetValue(KeyValuePairProperty); }
            set { SetValue(KeyValuePairProperty, value); }
        }

        public static readonly DependencyProperty KeyValuePairProperty = DependencyProperty.Register("KeyValuePair", typeof(object), typeof(SliderItemsControl), new PropertyMetadata(null));

        public object Dictionary
        {
            get { return (object)GetValue(DictionaryProperty); }
            set { SetValue(DictionaryProperty, value); }
        }

        public static readonly DependencyProperty DictionaryProperty = DependencyProperty.Register("Dictionary", typeof(object), typeof(SliderItemsControl), new PropertyMetadata(null));

        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IEnumerable), typeof(SliderItemsControl), new PropertyMetadata(null, Changed));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(SliderItemsControl), new PropertyMetadata(null, Changed));

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(SliderItemsControl), new PropertyMetadata(null, Changed));

        public string Min
        {
            get { return (string)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(string), typeof(SliderItemsControl), new PropertyMetadata(null, Changed));

        public string Max
        {
            get { return (string)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(string), typeof(SliderItemsControl), new PropertyMetadata(null, Changed));

        //private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as SliderItemsControl).dict[e.Property.Name].OnNext(e.NewValue);
        //}

        public SliderItemsControl()
        {
            SelectChanges(nameof(Data))
        .CombineLatest(SelectChanges(nameof(Key)),
         SelectChanges(nameof(Value)),
        SelectChanges(nameof(Min)).StartWith(new object[] { null }),
    SelectChanges(nameof(Max)).StartWith(new object[] { null }),
   (data, key, value, min, max) => new { data, key, value, min, max })
        .Subscribe(async _ =>
        {
            await (GetItems((IEnumerable)_.data, (string)_.key, (string)_.value, (string)_.min, (string)_.max)
            .ContinueWith(dp =>
           this.Dispatcher.InvokeAsync(async () =>
           {
               var xxx = (await dp).ToArray();
               Dictionary = xxx.ToDictionary(_a => _a.Key, _a => _a.Value);
               this.Dispatcher.Invoke(() =>
               {
                   KeyRangeCollection.Clear();
                   foreach (var xx in xxx)
                   {
                       xx.PropertyChanged += Xx_PropertyChanged;
                       KeyRangeCollection.Add(xx);
                   }
               });
           }, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken))));
        });

            SelectChanges(nameof(ShowKeyValuePanel)).Subscribe(_ =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    KeyValuePanel.Visibility = ((bool)_) ? Visibility.Visible : Visibility.Collapsed;
                });
            });
        }

        private Task<IEnumerable<KeyRange>> GetItems(IEnumerable data, string key, string value, string min, string max) => Task.Run(() =>
          {
              var type = data.First().GetType().GetProperties().ToDictionary(_ => _.Name, _ => _);
              var keys = UtilityHelper.PropertyHelper.GetPropertyValues<string>(data, type[key]);
              var values = data.Cast<object>().Select(_ =>
              type[value].GetValue(_)).ToList().Select(_ => Convert.ToDouble(_));
              var mins = min != null ? (type.TryGetValue(min, out PropertyInfo outmin) ?
             data.Cast<object>().Select(_ =>
              type[min].GetValue(_)).ToList().Select(_ => Convert.ToDouble(_)) : null) : null;
              var maxs = max != null ? (type.TryGetValue(min, out PropertyInfo outmax)) ?
                data.Cast<object>().Select(_ =>
              type[max].GetValue(_)).ToList().Select(_ => Convert.ToDouble(_)) : null : null;

              var xxx = Factory.Create(keys, values, mins, maxs);

              return xxx;
          });

        private void Xx_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                var keyValuePair = new KeyValuePair<string, double>((sender as KeyRange).Key, (sender as KeyRange).Value);
                KeyValuePair = keyValuePair;
                Dictionary = ItemsControl.ItemsSource.OfType<KeyRange>().ToDictionary(_ => _.Key, _ => _.Value);
                RaiseValueChangedEvent(keyValuePair);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SliderItemsControl));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        protected void RaiseValueChangedEvent(KeyValuePair<string, double> KeyValuePair)
        {
            KeyValuePairRoutedEventArgs newEventArgs = new KeyValuePairRoutedEventArgs(SliderItemsControl.ValueChangedEvent) { KeyValuePair = KeyValuePair };
            RaiseEvent(newEventArgs);
        }

        public class KeyValuePairRoutedEventArgs : RoutedEventArgs
        {
            public KeyValuePair<string, double> KeyValuePair { get; set; }

            public KeyValuePairRoutedEventArgs(RoutedEvent @event) : base(@event)
            {
            }
        }

        private class Factory
        {
            public static IEnumerable<KeyRange> Create(IEnumerable<string> keys, IEnumerable<double> values, IEnumerable<double> mins, IEnumerable<double> maxs)
            {
                using (var b = keys.GetEnumerator())
                using (var c = values.GetEnumerator())
                using (var d = mins?.GetEnumerator())
                using (var e = maxs?.GetEnumerator())
                {
                    while (b.MoveNext() && c.MoveNext() && (bool)(d?.MoveNext() ?? true) && (bool)(e?.MoveNext() ?? true))
                    {
                        var ff = new KeyRange(b.Current, c.Current, d?.Current, e?.Current);
                        yield return ff;
                    }
                }
            }
        }
    }

    public class KeyRange : INotifyPropertyChanged
    {
        private double value;

        public KeyRange(string key, double value, double? min = null, double? max = null)
        {
            Key = key;
            Value = value;
            Min = min ?? 0;
            Max = max ?? value * 2;
        }

        public int TickFrequency => (int)((Max - Min) / 10);
        public string Key { get; set; }
        public double Value { get { return value; } set { this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); } }
        public double Min { get; set; }
        public double Max { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}