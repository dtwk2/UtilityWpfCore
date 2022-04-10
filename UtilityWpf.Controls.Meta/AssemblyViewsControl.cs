using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf.Controls.Meta
{
    public class AssemblyViewsControl : ContentControl
    {
        public AssemblyViewsControl(/*Assembly[] assemblies*/)
        {
            var dockPanel = new DockPanel();
            var (comboBox, viewsDetailControl) = CreateChildren();
            dockPanel.Children.Add(comboBox);
            dockPanel.Children.Add(viewsDetailControl);
            Content = dockPanel;
        }

        private static (ComboBox comboBox, ViewsMasterDetailControl viewsDetailControl) CreateChildren()
        {
            var comboBox = new AssemblyComboBox();
            DockPanel.SetDock(comboBox, Dock.Top);
            Binding binding = new()
            {
                Path = new PropertyPath(nameof(ComboBox.SelectedValue)),
                Source = comboBox
            };
            comboBox.SelectionChanged += ComboBox_SelectionChanged;

            var viewsDetailControl = new ViewsMasterDetailControl { };
            BindingOperations.SetBinding(viewsDetailControl, ViewsMasterDetailControl.AssemblyProperty, binding);
            viewsDetailControl.DemoType = DemoType.ResourceDictionary;
            return (comboBox, viewsDetailControl);
        }

        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}