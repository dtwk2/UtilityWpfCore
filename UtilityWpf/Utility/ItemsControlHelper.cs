using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace UtilityWpf.Helper
{
    public static class ItemsControlHelper
    {
        public static IObservable<int> CountChanges(this ItemsControl headeredItemsControl)
        {
            return UtilityHelperEx.ObservableHelper.Pace(headeredItemsControl.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .Select(a => UtilityHelperEx.NonGeneric.ObservableHelper.ToObservable(a).ToObservableChangeSet()
                .ToCollection()
                .Select(a => a.Count)), TimeSpan.FromSeconds(0.3))
                .DistinctUntilChanged()
                .Switch();
        }
    }
}