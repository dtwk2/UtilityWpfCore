using System;
using System.Collections.ObjectModel;
using DynamicData;

namespace Utility.WPF.Controls.Date.Model;

public static class ListExt {
   public static void InsertInOrder<T>(this ObservableCollection<T> @this, T item) where T : IComparable<T> {

      if (@this.Count == 0) {
         @this.Add(item);
         return;
      }

      if (@this.Count > 0 && @this[^1].CompareTo(item) <= 0) {
         @this.Add(item);
         return;
      }

      if (@this[0].CompareTo(item) >= 0) {
         @this.Insert(0, item);
         return;
      }
      int index = @this.BinarySearch(item);
      if (index < 0)
         index = ~index;

      @this.Insert(index, item);
   }
}