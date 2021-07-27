using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityWpf.Controls.Panels
{
    public static class CombinationHelper
    {
        public static bool AllDistinct<T>(this ICollection<T> collection)
        {
            return collection.Count == collection.Distinct().Count();
        }

        public static IEnumerable<(T, T)> SelectCombinations<T>(this IList<T> coordinates)
        {
            int coordinatesIndex = coordinates.Count - 1;
            while (coordinatesIndex >= 0)
            {
                var last = coordinates[coordinatesIndex];
                yield return (last, last);

                foreach (var c in coordinates.Take(coordinatesIndex))
                {
                    yield return (last, c);
                    yield return (c, last);
                }
                coordinatesIndex--;
                coordinates.SelectCombinations();
            }
        }

        public static IEnumerable<IEnumerable<T>> SelectSetCombinations<T>(this IEnumerable<IEnumerable<T>> setOfSets)
        {

            using var enumr = setOfSets.GetEnumerator();
            enumr.MoveNext();
            if (enumr.Current == null)
                return Array.Empty<IEnumerable<T>>();

            IEnumerable<IEnumerable<T>> combinations = enumr.Current.Select(a => new[] { a });
            while (enumr.MoveNext())
                try
                {
                    combinations = AddToCollection(combinations, enumr.Current);
                }
                catch (Exception)
                {

                }
            return combinations;

            static IEnumerable<IEnumerable<T>> AddToCollection(IEnumerable<IEnumerable<T>> elementSets, IEnumerable<T> newElements)
            {
                foreach (var elements in elementSets)
                    foreach (var newElement in newElements)
                        yield return elements.Concat(new[] { newElement });
            }
        }
    }
}
