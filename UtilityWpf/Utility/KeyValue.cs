using System.Collections.Generic;
using System.ComponentModel;
using ReactiveUI;

namespace UtilityWpf.Property
{
    public class KeyValue
    {
        public KeyValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }
    }

    /// <summary>
    /// From <a href="https://www.broculos.net/2014/03/wpf-editable-datagrid-and.html"></a>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReactivePair<TKey, TValue> : ReactiveObject
    {
        protected TKey key;
        protected TValue value;

        public ReactivePair()
        {
        }

        public ReactivePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public ReactivePair(KeyValuePair<TKey, TValue> kv)
        {
            Key = kv.Key;
            Value = kv.Value;
        }

        public TKey Key
        {
            get => key;
            set => this.RaiseAndSetIfChanged(ref this.key, value);
        }

        public TValue Value
        {
            get => value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
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