using System;
using System.Collections.Generic;

namespace Utility.Common.Helper
{
    public static class DictionaryHelper
    {
        public static IEnumerable<(TKey key, TValue one, TValue two)> Differences<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary1,
            IDictionary<TKey, TValue> dictionary2) => Compare(dictionary1, dictionary2, false);
        public static IEnumerable<(TKey key, TValue one, TValue two)> Similarities<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary1,
        IDictionary<TKey, TValue> dictionary2) => Compare(dictionary1, dictionary2, true);

        public static IEnumerable<(TKey key, TValue one, TValue two)> Compare<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary1,
            IDictionary<TKey, TValue> dictionary2,
            bool match)
        {
            foreach (var key in dictionary1.Keys)
            {
                var one = dictionary1.GetValueOrDefault(key);
                var two = dictionary2.GetValueOrDefault(key);
                if (one is not null && two is not null && one.Equals(two).Equals(match) == true)
                {
                    yield return (key, one, two);
                }
            }
        }

        public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
