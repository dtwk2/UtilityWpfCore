using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Model;

namespace UtilityWpf.Controls.Meta
{
    public class AssemblyViewsControl : ContentControl
    {
        public AssemblyViewsControl(Type[] types) : this(types.Select(t => t.Assembly).ToArray())
        {
        }

        public AssemblyViewsControl(Assembly[] assemblies)
        {
            Content = CreateContent(out var comboBox);
            comboBox.ItemsSource = assemblies.Select(a => new ViewAssembly(a)).ToArray();
        }

        private static object CreateContent(out ComboBox comboBox)
        {
            var dockPanel = new DockPanel();
            comboBox = new AssemblyComboBox();
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
    }

    internal class AssemblyComboBox : ComboBox
    {
        public AssemblyComboBox()
        {
            SelectedIndex = 0;
            FontWeight = FontWeights.DemiBold;
            FontSize = 14;
            Margin = new Thickness(4);
            Width = 700;
            Height = 30;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            BindingOperations.SetBinding(element, ComboBoxItem.ContentProperty, new Binding(nameof(ViewAssembly.Key)));
            base.PrepareContainerForItemOverride(element, item);
        }
    }
}