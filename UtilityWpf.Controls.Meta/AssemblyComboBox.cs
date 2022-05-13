using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Common;
using Utility.Persist;
using UtilityInterface.NonGeneric;
using UtilityWpf.Controls.Buttons;
using UtilityWpf.Model;
using UtilityWpf.Helper;

namespace UtilityWpf.Controls.Meta
{
    using static UtilityWpf.DependencyPropertyFactory<AssemblyComboBox>;

    public record AssemblyRecord(string Key, DateTime Inserted);

    public class AssemblyEntity : BaseEntity<AssemblyEntity, Guid>, IKey
    {
        public string Key { get; init; }
        public bool IsSelected { get; set; }
        public bool IsChecked { get; set; }
    }

    internal class AssemblyComboBox : CheckBoxesComboControl
    {
        private readonly Subject<DemoType> subject = new();

        public static readonly DependencyProperty DemoTypeProperty = Register(nameof(DemoType), a => a.subject);

        public AssemblyComboBox()
        {
            //SelectedIndex = 0;
            FontWeight = FontWeights.DemiBold;
            FontSize = 14;
            Margin = new Thickness(4);
            Width = 1000;
            Height = 60;
            DisplayMemberPath = nameof(AssemblyKeyValue.Key);
            IsCheckedPath = nameof(Model.KeyValue.IsChecked);
            IsSelectedPath = nameof(Model.KeyValue.IsSelected);
            SelectedValuePath = nameof(AssemblyKeyValue.Value);

            FreeSqlFactory.InitialiseSQLite();

            subject
                //.StartWith(DemoType.ResourceDictionary)
                .Select(a =>
                {
                    return a switch
                    {
                        DemoType.UserControl => FindDemoAppAssemblies(),
                        DemoType.ResourceDictionary => FindResourceDictionaryAssemblies(),
                        _ => throw new Exception("££!!!!$$4"),
                    };
                })
                .Subscribe(async a =>
                {
                    //    var array2 = a.Select(a => new AssemblyKeyValue(a))
                    //.Where(a => a.Key != null)
                    //.Select(a => A<AssemblyEntity>.Order(a.Key))
                    //.ToArray();

                    var array = a
                    .Select(a => new AssemblyKeyValue(a))
                    .Where(a => a.Key != null)
                    .OrderByDescending(a => BaseEntityOrderer<AssemblyEntity>.Order(a.Key))
                    .ToArray();

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
                    ItemsSource = view;
                    //SelectedIndex = 0;
                });

            //GroupStyle.Add(new GroupStyle());

            this.SelectSingleSelectionChanges()
                .DistinctUntilChanged()
                .Subscribe(async a =>
                {
                    var item = (AssemblyKeyValue)a;

                    if (item.Key != null)
                    {
                        var match = await AssemblyEntity.Where(a => a.Key == item.Key).FirstAsync();
                        if (match == null)
                        {
                            //repo.Add(new AssemblyRecord(item.Key, DateTime.Now));
                            var assemblyEntity = new AssemblyEntity { Key = item.Key, IsSelected = true, IsChecked = true };
                            await assemblyEntity.InsertAsync();
                        }
                        else if (match.IsSelected != true)
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
                        }
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        SelectedItem = a;
                    });
                });

            this.SelectOutputChanges()
                .Subscribe(async a =>
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
                                await match.UpdateAsync();
                            }
                            else
                            {
                                throw new Exception("gref34 gdfg");
                            }
                        }
                    }
                });
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

        //protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        //{
        //    BindingOperations.SetBinding(element, ComboBoxItem.ContentProperty, new Binding(nameof(ViewAssembly.Key)));
        //    base.PrepareContainerForItemOverride(element, item);
        //}

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

        public DemoType DemoType
        {
            get => (DemoType)GetValue(DemoTypeProperty);
            set => SetValue(DemoTypeProperty, value);
        }
    }
}