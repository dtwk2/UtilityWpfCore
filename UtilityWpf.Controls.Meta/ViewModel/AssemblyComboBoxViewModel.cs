using Evan.Wpf;
using FreeSql;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows;
using Utility.Common;
using Utility.Persist;
using UtilityWpf.Model;
using System.Reactive;
using System.ComponentModel;
using System.Reactive.Linq;
using UtilityInterface.NonGeneric;
using System.Windows.Controls;
using System.Reactive.Threading.Tasks;
using Utility.Common.Model;

namespace UtilityWpf.Controls.Meta.ViewModel
{
    internal class AssemblyComboBoxViewModel
    {
        public static readonly DependencyProperty DemoTypeProperty = DependencyHelper.Register();

        public Connection<DemoType, ICollectionView> demoTypeViewModel;
        public Connection<object, object> selectedItemViewModel;
        public Connection<Buttons.Infrastructure.CheckedRoutedEventArgs, Unit> checkedViewModel;

        public AssemblyComboBoxViewModel()
        {
            FreeSqlFactory.InitialiseSQLite();

            demoTypeViewModel = Connection<DemoType, ICollectionView>.Create(@in =>

              @in
                .Select(a =>
                {
                    return a switch
                    {
                        DemoType.UserControl => FindDemoAppAssemblies(),
                        DemoType.ResourceDictionary => FindResourceDictionaryAssemblies(),
                        _ => throw new Exception("££!!!!$$4"),
                    };
                })
                .Select(async a =>
                {
                    //    var array2 = a.Select(a => new AssemblyKeyValue(a))
                    //.Where(a => a.Key != null)
                    //.Select(a => A<AssemblyEntity>.Order(a.Key))
                    //.ToArray();

                    AssemblyKeyValue[] array = ToKeyValues(a);
                    var items = await AssemblyEntity.Select.ToListAsync();
                    for (int i = 0; i < array.Length; i++)
                    {
                        foreach (var item in items)
                            if (array[i].Key == item.Key)
                            {
                                array[i].IsSelected = item.IsSelected;
                                array[i].IsChecked = item.IsChecked;
                            }
                    }

                    var view = CollectionViewSource.GetDefaultView(array);
                    //view.GroupDescriptions.Clear();
                    //view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Model.KeyValue.GroupKey)));
                    return view;
                    //SelectedIndex = 0;
                })
                .SelectMany(a => a.ToObservable())
            );

            selectedItemViewModel = Connection<object, object>.Create(@in =>
            @in
                .Select(async a =>
                {
                    var item = (AssemblyKeyValue)a;

                    if (item.Key != null)
                    {
                        var match = await AssemblyEntity.Where(a => a.Key == item.Key).FirstAsync();
                        if (match == null)
                        {
                            //repo.Add(new AssemblyRecord(item.Key, DateTime.Now));
                            var assemblyEntity = new AssemblyEntity { Key = item.Key, IsChecked = true };
                            await assemblyEntity.InsertAsync();
                            SelectAndUpdateOtherSelections(assemblyEntity);
                        }
                        else if (match.IsSelected != true)
                        {
                            SelectAndUpdateOtherSelections(match);
                        }
                    }
                    return item;
                })
                .SelectMany(a => a.ToObservable()));



            checkedViewModel = Connection<Buttons.Infrastructure.CheckedRoutedEventArgs, Unit>.Create(@in =>

                @in
                .Select(async a =>
                {
                    var enumerator = a.Dictionary.GetEnumerator();
                    while (enumerator.MoveNext() && enumerator.Current is { Key: var key, New: var @new })
                    {
                        var match = await AssemblyEntity.Where(a => a.Key == key).FirstAsync();
                        if (match == null)
                        {
                            var assemblyEntity = new AssemblyEntity { Key = key.ToString(), IsSelected = true, IsChecked = true };
                            await assemblyEntity.InsertAsync();
                        }
                        else
                        {
                            if (@new.HasValue)
                            {
                                match.IsChecked = @new ?? false;
                                if (match.IsChecked == false)
                                    match.IsSelected = false;
                                await match.UpdateAsync();
                            }
                            else
                            {
                                throw new Exception("gref34 gdfg");
                            }
                        }
                    }
                    return Unit.Default;
                })
                .SelectMany(a => a.ToObservable()));
        }

        private static AssemblyKeyValue[] ToKeyValues(IEnumerable<Assembly> a)
        {
            return a
            .Select(a => new AssemblyKeyValue(a))
            .Where(a => a.Key != null)
            .OrderByDescending(a => BaseEntityOrderer<AssemblyEntity>.Order(a.Key))
            .ToArray();
        }

        private class BaseEntityOrderer<T> where T : BaseEntity, IKey
        {
            public static DateTime Order(string key)
            {
                var where = BaseEntity.Orm.Select<T>().Where(a => a.Key == key);
                var match = where.MaxAsync(a => a.UpdateTime);
                if (match.Result == default)
                {
                    return where.MaxAsync(a => a.CreateTime).Result;
                }
                return match.Result;
            }
        }

        public record AssemblyRecord(string Key, DateTime Inserted);

        public class AssemblyEntity : BaseEntity<AssemblyEntity, Guid>, IKey
        {
            public string Key { get; init; }
            public bool IsSelected { get; set; }
            public bool IsChecked { get; set; }
        }

        private const string DemoAppNameAppendage = "Demo";

        private static IEnumerable<Assembly> FindDemoAppAssemblies()
        {
            return from a in AssemblySingleton.Instance.Assemblies
                   where a.GetName().Name.Contains(DemoAppNameAppendage)
                   where a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(UserControl)))
                   select a;
        }

        private static IEnumerable<Assembly> FindResourceDictionaryAssemblies(Predicate<string>? predicate = null)
        {
            return from a in AssemblySingleton.Instance.Assemblies
                   let resNames = a.GetManifestResourceNames()
                   where resNames.Length > 0
                   select a;
        }

        private static async void SelectAndUpdateOtherSelections(AssemblyEntity match)
        {
            var matches = await AssemblyEntity.Where(a => a.IsSelected).ToListAsync();
            foreach (var match2 in matches)
            {
                if (match2 != match)
                {
                    match2.IsSelected = false;
                    await match2.UpdateAsync();
                }
            }
            match.IsSelected = true;
            await match.UpdateAsync();

            var count = await AssemblyEntity.WhereIf(true, a => a.IsSelected).CountAsync();
            if (count != 1)
            {
                throw new Exception("Expected count to be 1 since only item can be selected in any given moment");
            }            
        }
    }
}
