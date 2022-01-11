using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper;

namespace UtilityWpf.Model
{
    public class ViewType
    {
        private Lazy<FrameworkElement?> lazy;

        public ViewType(string key, Type type)
        {
            Key = key;
            Type = type;
            lazy = new(() => (FrameworkElement?)Activator.CreateInstance(Type));
        }

        public string Key { get; }

        public Type Type { get; }

        public FrameworkElement? View => lazy.Value;

        public static IEnumerable<ViewType> ViewTypes(Assembly assembly) => assembly
       .GetTypes()
       .Where(a => typeof(UserControl).IsAssignableFrom(a))
       .GroupBy(type =>
       (type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
       type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
       type.Name)!)
       .OrderBy(a => a.Key)
       .ToDictionaryOnIndex()
       .Select(a => new ViewType(a.Key, a.Value));
    }

    internal static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
             => groupings
            .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
           .ToDictionary(a => a.Key, a => a.Value);
    }
}