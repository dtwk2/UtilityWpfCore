﻿using NetFabric.Hyperlinq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Base;
using UtilityWpf.Controls.Master;
using UtilityWpf.Model;

namespace UtilityWpf.Controls.Meta
{
    using static UtilityWpf.DependencyPropertyFactory<ViewsMasterDetailControl>;

    internal class ViewTypeItem : ListBoxItem
    {
    }

    internal class ViewTypeItemListBox : ListBox<ViewTypeItem>
    {
    }

    public enum DemoType
    {
        UserControl,
        ResourceDictionary
    }

    public class ViewsMasterDetailControl : MasterDetail
    {
        private readonly Subject<Assembly> subject = new();
        private readonly Subject<DemoType> subject2 = new();
        public static readonly DependencyProperty AssemblyProperty = Register(nameof(Assembly), a => a.subject, initialValue: Assembly.GetEntryAssembly());
        public static readonly DependencyProperty DemoTypeProperty = Register(nameof(DemoType), a => a.subject2);

        public ViewsMasterDetailControl()
        {
            Orientation = Orientation.Horizontal;
            var listBox = new ViewTypeItemListBox();

            //listBox.GroupStyle.Add(new GroupStyle());

            //var resource = new ResourceDictionary
            //{
            //    Source = new Uri("/UtilityWpf.Controls.Meta;component/Themes/Generic.xaml",
            //         UriKind.RelativeOrAbsolute)
            //};
            //var dataTemplateKey = new DataTemplateKey(typeof(KeyValue));
            //listBox.ItemTemplate = (DataTemplate)resource[dataTemplateKey];
            Content = listBox;
            UseDataContext = true;
            _ = subject
                .StartWith(Assembly)
                .WhereNotNull()
                .CombineLatest(subject2)
                .Select(a =>
                {
                    return a.Second switch
                    {
                        DemoType.UserControl => FrameworkElementKeyValue.ViewTypes(a.First),
                        DemoType.ResourceDictionary => (IEnumerable<KeyValue>)ResourceDictionaryKeyValue.ResourceViewTypes(a.First),
                        _ => throw new Exception("FDGS££Fff"),
                    };
                })
                .Subscribe(pairs =>
                {
                    listBox.ItemsSource = pairs.ToArray();
                    listBox.SelectedIndex = 0;
                });
            //subject2.OnNext(DemoType.UserControl);

            Header = CreateDetail();

            static Grid CreateDetail()
            {
                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                var textBlock = new TextBlock
                {
                    Margin = new Thickness(20),
                    FontSize = 20,
                    Text = "e m p t y"
                };
                grid.Children.Add(textBlock);
                Binding binding = new()
                {
                    Path = new PropertyPath(nameof(KeyValue.Key)),
                };
                textBlock.SetBinding(TextBlock.TextProperty, binding);
                var contentControl = new ContentControl
                {
                    Content = new TextBlock { Text = "e m p t y", FontSize = 30, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center },
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                Grid.SetRow(contentControl, 1);
                binding = new Binding
                {
                    Path = new PropertyPath(nameof(KeyValue.Value)),
                };
                contentControl.SetBinding(ContentProperty, binding);
                grid.Children.Add(contentControl);
                return grid;
            }
        }

        public Assembly Assembly
        {
            get => (Assembly)GetValue(AssemblyProperty);
            set => SetValue(AssemblyProperty, value);
        }

        public DemoType DemoType
        {
            get => (DemoType)GetValue(DemoTypeProperty);
            set => SetValue(DemoTypeProperty, value);
        }
    }
}