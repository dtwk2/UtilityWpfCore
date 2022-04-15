using System;
using System.Collections.ObjectModel;
using DynamicData;

namespace Utility.WPF.Controls.Date.Model;

public static class ListExt
{
    public static void InsertInOrder<T>(this ObservableCollection<T> collection, T item) where T : IComparable<T>
    {

        if (collection.Count == 0)
        {
            collection.Add(item);
            return;
        }

        if (collection.Count > 0 && collection[^1].CompareTo(item) <= 0)
        {
            collection.Add(item);
            return;
        }

        if (collection[0].CompareTo(item) >= 0)
        {
            collection.Insert(0, item);
            return;
        }
        int index = collection.BinarySearch(item);
        if (index < 0)
            index = ~index;

        collection.Insert(index, item);
    }

    public static void InsertInOrderIfMissing<T>(this ObservableCollection<T> collection, params T[] set)
        where T : IEquatable<T>, IComparable<T>
    {
        foreach (var item in set)
        {
            if (collection.Contains(item))
            {
                continue;
            }

            collection.InsertInOrder(item);
        }
    }
}