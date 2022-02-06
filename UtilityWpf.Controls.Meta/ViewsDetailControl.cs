using ReactiveUI;
using System;
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
    using static UtilityWpf.DependencyPropertyFactory<ViewsDetailControl>;

    internal class ViewTypeItem : ListBoxItem
    {
    }

    internal class ViewTypeItemListBox : ListBox<ViewTypeItem>
    {
    }

    public class ViewsDetailControl : MasterDetail
    {
        private readonly Subject<Assembly> subject = new();
        public static readonly DependencyProperty AssemblyProperty = Register(nameof(Assembly), a => a.subject, initialValue: Assembly.GetEntryAssembly());

        public ViewsDetailControl()
        {
            Orientation = Orientation.Horizontal;
            var listBox = new ViewTypeItemListBox();
            Content = listBox;
            UseDataContext = true;
            _ = subject
                .StartWith(Assembly)
                .WhereNotNull()
                .Select(ViewType.ViewTypes)
                .Subscribe(pairs =>
                {
                    listBox.ItemsSource = pairs;
                    listBox.SelectedIndex = 0;
                });

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
                    Path = new PropertyPath(nameof(ViewType.Key)),
                };
                textBlock.SetBinding(TextBlock.TextProperty, binding);
                var contentControl = new ContentControl { 
              
                    Content = new TextBlock{Text="e m p t y", FontSize = 30, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center}, 
                    VerticalAlignment = VerticalAlignment.Stretch, 
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
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
            get => (Assembly)GetValue(AssemblyProperty);
            set => SetValue(AssemblyProperty, value);
        }
    }
}