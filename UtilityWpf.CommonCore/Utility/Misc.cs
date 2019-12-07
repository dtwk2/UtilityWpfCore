using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UtilityWpf
{
    public static class Misc
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }
    }
}