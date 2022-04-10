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
using UtilityWpf.Model;
using UtilityWpf.Utility;

namespace UtilityWpf.Controls.Meta
{
    using static UtilityWpf.DependencyPropertyFactory<AssemblyComboBox>;

    public record AssemblyRecord(string Key, DateTime Accessed);

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
            var repo = new LiteDbRepository(new(typeof(AssemblyRecord), "AssemblyRecords"));
            var aa = new A(repo);
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
                    var array = a.Select(a => new AssemblyKeyValue(a)).OrderBy(a => aa.Order(a.Key)).ToArray();
                    var view = CollectionViewSource.GetDefaultView(array);
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UtilityWpf.Model.KeyValue.GroupKey)));
                    ItemsSource = view;
                    //ItemsSource = a.Select(a => new AssemblyKeyValue(a)).ToArray();
                });
            GroupStyle.Add(new GroupStyle());
            this.SelectSingleSelectionChanges()
                .Subscribe(a =>
                {
                    var item = (AssemblyKeyValue)a;
                    if (item.Key != null)
                        repo.Add(new AssemblyRecord(item.Key, DateTime.Now));
                });
        }

        private class A
        {
            private readonly LiteDbRepository repo;

            public A(LiteDbRepository repo)
            {
                this.repo = repo;
            }

            public int Order(string key)
            {
                //var match = repo.FindBy(new MatchesStringQuery(key, nameof(AssemblyRecord.Key), AbsoluteOrder.Last));
                //if (match != null)
                //{
                //}
                return 0;
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