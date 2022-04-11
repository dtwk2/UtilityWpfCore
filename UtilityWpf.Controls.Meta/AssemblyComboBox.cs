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
using UtilityWpf.Model;
using UtilityWpf.Utility;

namespace UtilityWpf.Controls.Meta
{
    using static UtilityWpf.DependencyPropertyFactory<AssemblyComboBox>;

    public record AssemblyRecord(string Key, DateTime Inserted);

    public class AssemblyEntity : BaseEntity<AssemblyEntity, Guid>, IKey
    {
        public string Key { get; init; }
    }

    internal class AssemblyComboBox : ComboBox
    {
        private readonly Subject<DemoType> subject = new();

        public static readonly DependencyProperty DemoTypeProperty = Register(nameof(DemoType), a => a.subject);

        public AssemblyComboBox()
        {
            SelectedIndex = 0;
            FontWeight = FontWeights.DemiBold;
            FontSize = 14;
            Margin = new Thickness(4);
            Width = 700;
            Height = 30;
            DisplayMemberPath = nameof(AssemblyKeyValue.Key);
            SelectedValuePath = nameof(AssemblyKeyValue.Value);

            FreeSqlFactory.InitialiseSQLite();

            subject
                .StartWith(DemoType.ResourceDictionary)
                .Select(a =>
                {
                    return a switch
                    {
                        DemoType.UserControl => FindDemoAppAssemblies(),
                        DemoType.ResourceDictionary => FindResourceDictionaryAssemblies(),
                        _ => throw new Exception("££!!!!$$4"),
                    };
                })
                .Subscribe(a =>
                {
                    //    var array2 = a.Select(a => new AssemblyKeyValue(a))
                    //.Where(a => a.Key != null)
                    //.Select(a => A<AssemblyEntity>.Order(a.Key))
                    //.ToArray();

                    var array = a.Select(a => new AssemblyKeyValue(a))
                    .Where(a => a.Key != null)
                    .OrderByDescending(a => A<AssemblyEntity>.Order(a.Key))
                    .ToArray();

                    var view = CollectionViewSource.GetDefaultView(array);
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UtilityWpf.Model.KeyValue.GroupKey)));
                    ItemsSource = array;
                    SelectedIndex = 0;
                });

            GroupStyle.Add(new GroupStyle());

            this.SelectSingleSelectionChanges()
                .Subscribe(async a =>
                {
                    var item = (AssemblyKeyValue)a;
                    if (item.Key != null)
                    {
                        var assemblyEntity = new AssemblyEntity { Key = item.Key };
                        var sort = assemblyEntity.Sort;
                        var x = await AssemblyEntity.Where(a => a.Key == item.Key).FirstAsync();
                        if (x == null)
                            //repo.Add(new AssemblyRecord(item.Key, DateTime.Now));

                            await assemblyEntity.InsertAsync();
                        else
                            await x.UpdateAsync();
                    }
                });
        }

        private class A<T> where T : BaseEntity, IKey
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