using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace UtilityWpf.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var listofLists = Enumerable.Range(0, 4).Select((a, i) => Enumerable.Range(0, 5 - i).ToArray()).ToArray();
            var combos = SelectCombinations(listofLists).ToArray();
            Assert.Pass();
        }

        static IEnumerable<IEnumerable<T>> SelectCombinations<T>(IEnumerable<IList<T>> setOfSets)
        {
            using var enumr = setOfSets.GetEnumerator();
            enumr.MoveNext();
            IEnumerable<IEnumerable<T>> combinations = enumr.Current.Select(a => new[] { a });
            while (enumr.MoveNext())
                combinations = AddToCollection(combinations, enumr.Current);

            return combinations;

            static IEnumerable<IEnumerable<T>> AddToCollection<T>(IEnumerable<IEnumerable<T>> elementSets, IEnumerable<T> newElements)
            {
                foreach (var elements in elementSets)
                    foreach (var newElement in newElements)
                        yield return elements.Concat(new[] { newElement });
            }
        }
    }
}