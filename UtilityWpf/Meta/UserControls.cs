using MoreLinq;
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
using UtilityHelper;
using UtilityWpf.Model;

namespace UtilityWpf.Meta
{
    using static UtilityWpf.DependencyPropertyFactory<UserControls>;
    /// <summary>
    /// Master-Detail control for <see cref="UserControls"/> in <seet cref="UserControls.Assembly"></set>
    /// </summary>
    public class UserControls : Grid
    {
        readonly Subject<Assembly> subject = new();
        public static readonly DependencyProperty AssemblyProperty = Register(nameof(Assembly), a => a.subject, initialValue: Assembly.GetEntryAssembly());

        public UserControls(Assembly? assembly = null)
        {
            if (assembly != null)
                Assembly = assembly;

            var listBox = new ViewListBox();
            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
            this.Children.Add(listBox);

            _ = subject
                .StartWith(Assembly)
                .WhereNotNull()
                .Select(assembly => ViewType.ViewTypes(assembly))
                .Subscribe(pairs => listBox.ItemsSource = pairs);

            this.Children.Add(ControlFactory.CreateMainGrid(listBox));
        }

        public Assembly Assembly
        {
            get { return (Assembly)GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }
    }

    class ViewListBox : ListBox
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            InitialiseItem(element as ListBoxItem);
            base.PrepareContainerForItemOverride(element, item);

            static void InitialiseItem(ListBoxItem? item)
            {
                item.SetBinding(ListBoxItem.ContentProperty, new Binding
                {
                    Path = new PropertyPath(nameof(ViewType.Key)),
                });
            }
        }
    }

    static class ControlFactory
    {
        public static Grid CreateMainGrid(ListBox listBox)
        {
            var grid = new Grid();
            Grid.SetColumn(grid, 1);
            int i = 0;
            foreach (var (item, gut) in CreateContent(CreateBinding(listBox)))
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, gut) });
                grid.Children.Add(item);
                Grid.SetRow(item, i);
                i++;
            }
            return grid;

            static Binding CreateBinding(ListBox listBox)
            {
                return new Binding
                {
                    Path = new PropertyPath(nameof(ListBox.SelectedItem)),
                    Source = listBox,
                };
            }

            static IEnumerable<(FrameworkElement, GridUnitType)> CreateContent(Binding selectedItemBinding)
            {
                yield return (CreateTextBlock(selectedItemBinding), GridUnitType.Auto);
                yield return (CreateContentControl(selectedItemBinding), GridUnitType.Star);

                static TextBlock CreateTextBlock(Binding selectedItemBinding)
                {
                    TextBlock textBlock = new()
                    {
                        Margin = new Thickness(20),
                        FontSize = 20
                    };
                    _ = textBlock.SetBinding(TextBlock.TextProperty, new Binding()
                    {
                        Path = new PropertyPath(nameof(ViewType.Key)),
                    });
                    _ = textBlock.SetBinding(ContentControl.DataContextProperty, selectedItemBinding);
                    return textBlock;
                }

                static ContentControl CreateContentControl(Binding selectedItemBinding)
                {
                    ContentControl contentControl = new() { Content = "Empty" };
                    _ = contentControl.SetBinding(ContentControl.ContentProperty, new Binding
                    {
                        Path = new PropertyPath(nameof(ViewType.View)),
                    });
                    _ = contentControl.SetBinding(ContentControl.DataContextProperty, selectedItemBinding);
                    return contentControl;
                }
            }
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
