using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Controls.Master;
using UtilityWpf.Model;

namespace UtilityWpf.Controls.Meta
{
    using static UtilityWpf.DependencyPropertyFactory<ViewsDetailControl>;

    class ViewTypeItem : ListBoxItem
    {
    }

    class ViewTypeItemListBox : ListBox<ViewTypeItem>
    {
    }

    public class ViewsDetailControl : MasterDetail
    {
        readonly Subject<Assembly> subject = new();
        public static readonly DependencyProperty AssemblyProperty = Register(nameof(Assembly), a => a.subject, initialValue: Assembly.GetEntryAssembly());

        public ViewsDetailControl()
        {

            var listBox = new ViewTypeItemListBox();
            Selector = listBox;
            UseDataContext = true;
            _ = subject
                .StartWith(Assembly)
                .WhereNotNull()
              .Select(assembly =>
              {
                  return ViewType.ViewTypes(assembly);
              })
              .Subscribe(pairs =>
              {
                  listBox.ItemsSource = pairs;
                  listBox.SelectedIndex = 0;

              });

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
}


