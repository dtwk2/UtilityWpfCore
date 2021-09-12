using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MoreLinq;
using ReactiveUI;
using UtilityHelper;

namespace UtilityWpf.Controls.Master
{
    using static UtilityWpf.DependencyPropertyFactory<ViewsDetailControl>;

    class ViewType
    {
        Lazy<FrameworkElement> lazy;
        public ViewType(string key, Type type)
        {
            Key = key;
            Type = type;
            lazy = new(() => (FrameworkElement)Activator.CreateInstance(Type));
        }

        public string Key { get; }

        public Type Type { get; }

        public FrameworkElement DirtyView => lazy.Value;

        public FrameworkElement View => (FrameworkElement)Activator.CreateInstance(Type);
    }

    class ViewAssembly
    {
        public ViewAssembly(Assembly assembly)
        {
            Assembly = assembly;
        }

        public string Key => Assembly.FullName;
        public Assembly Assembly { get; }
    }

    public class ViewsExDetailControl : ContentControl
    {
        public ViewsExDetailControl(Type[] types)
        {
            Content = CreateContent(out var comboBox);
            comboBox.ItemsSource = types.Select(a => new ViewAssembly(a.Assembly));
        }

        private static object CreateContent(out ComboBox comboBox)
        {
            var dockPanel = new DockPanel();
            comboBox = new ComboBox { SelectedIndex = 0, Margin = new Thickness(5), Width = 700 };
            dockPanel.Children.Add(comboBox);
            DockPanel.SetDock(comboBox, Dock.Top);
            var viewsDetailControl = new ViewsDetailControl { };
            Binding binding = new()
            {
                Path = new PropertyPath(nameof(ComboBox.SelectedItem) + "." + nameof(ViewAssembly.Assembly)),
                Source = comboBox
            };
            BindingOperations.SetBinding(viewsDetailControl, ViewsDetailControl.AssemblyProperty, binding);
            dockPanel.Children.Add(viewsDetailControl);

            return dockPanel;

        }

        //      <DockPanel>
        //    <ComboBox x:Name="AssemblyComboBox" DockPanel.Dock="Top" SelectedIndex="0" Margin="5" Width="700">
        //        <ComboBox.ItemTemplate>
        //            <DataTemplate>
        //                <TextBlock Text = "{Binding FullName}" ></ TextBlock >
        //            </ DataTemplate >
        //        </ ComboBox.ItemTemplate >
        //    </ ComboBox >
        //    < view:ViewsDetailControl x:Name="HostUserControl" Assembly="{Binding ElementName=AssemblyComboBox, Path=SelectedItem}" />
        //</DockPanel>
    }



    public class ViewsDetailControl : MasterDetail
    {
        readonly Subject<Assembly> subject = new();
        public static readonly DependencyProperty AssemblyProperty = Register(nameof(Assembly), a => a.subject, initialValue: Assembly.GetEntryAssembly());

        public ViewsDetailControl()
        {

            var listBox = new ListBox();
            Selector = listBox;
            UseDataContext = true;
            _ = subject
                .WhereNotNull()
              .Select(assembly =>
              {
                  var ucs = assembly
                   .GetTypes()
                   .Where(a => typeof(UserControl).IsAssignableFrom(a))
                   .GroupBy(type =>
                   (type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
                   type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
                   type.Name)!)
                   .OrderBy(a => a.Key)
                   .ToDictionaryOnIndex()
                   .Select(a => new ViewType(a.Key, a.Value));
                  return ucs.ToArray();
              })
              .Subscribe(pairs => listBox.ItemsSource = pairs);

            Content = CreateContent();


            static Grid CreateContent()
            {
                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                var textBlock = new TextBlock
                {
                    Margin = new Thickness(20),
                    FontSize = 20
                };
                grid.Children.Add(textBlock);
                Binding binding = new()
                {
                    Path = new PropertyPath(nameof(ViewType.Key)),
                };
                textBlock.SetBinding(TextBlock.TextProperty, binding);
                var contentControl = new ContentControl { Content = "Empty" };
                Grid.SetRow(contentControl, 1);
                binding = new Binding
                {
                    Path = new PropertyPath(nameof(ViewType.View)),

                };
                contentControl.SetBinding(ContentProperty, binding);
                grid.Children.Add(contentControl);
                return grid;
            }

        }


        public Assembly Assembly
        {
            get { return (Assembly)GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }


    }

    public static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
           .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
          .ToDictionary(a => a.Key, a => a.Value);
    }
}


