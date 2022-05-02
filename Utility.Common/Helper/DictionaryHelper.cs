using System.Collections.Generic;

namespace Utility.Common.Helper
{
    public static class DictionaryHelper
    {
        public static IEnumerable<(TKey key, TValue one, TValue two)> Differences<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary1,
            IDictionary<TKey, TValue> dictionary2)=> Compare(dictionary1, dictionary2, false);
        public static IEnumerable<(TKey key, TValue one, TValue two)> Similarities<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary1,
        IDictionary<TKey, TValue> dictionary2) => Compare(dictionary1, dictionary2, true);

        public static IEnumerable<(TKey key, TValue one, TValue two)> Compare<TKey, TValue>(
 this IDictionary<TKey, TValue> dictionary1,
 IDictionary<TKey, TValue> dictionary2, bool match)
        {
            foreach (var key in dictionary1.Keys)
            {
                var one = dictionary1[key];
                var two = dictionary2[key];
                if (one.Equals(two).Equals(match))
                {
                    yield return (key, one, two);
                }
            }
        }
    }
}
