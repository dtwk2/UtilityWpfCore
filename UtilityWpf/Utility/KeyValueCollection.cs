namespace UtilityWpf.Property
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class ObservablePairCollection<TKey, TValue> : ObservableCollection<ReactivePair<TKey, TValue>>
    {
        public EventHandler KeyValueChanged;

        public ObservablePairCollection() : base()
        {
        }

        public ObservablePairCollection(IEnumerable<ReactivePair<TKey, TValue>> enumerable)
            : base(enumerable)
        {
        }

        public ObservablePairCollection(List<ReactivePair<TKey, TValue>> list)
            : base(list)
        {
        }

        public ObservablePairCollection(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var kv in dictionary)
            {
                Add(kv.Key, kv.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            var pair = new ReactivePair<TKey, TValue>(key, value);
            pair.PropertyChanged += (a, b) => Pair_PropertyChanged(key);
            Add(pair);
        }

        private void Pair_PropertyChanged(TKey key)
        {
            KeyValueChanged?.Invoke(this, new PropertyChangedEventArgs(key.ToString()));
        }
    }
}