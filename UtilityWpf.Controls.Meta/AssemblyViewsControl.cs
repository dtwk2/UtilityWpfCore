using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Controls.Buttons;

namespace UtilityWpf.Controls.Meta
{
    public class AssemblyViewsControl : ContentControl
    {
        public AssemblyViewsControl(/*Assembly[] assemblies*/)
        {
            var dockPanel = new DockPanel();
            var (comboBox, viewsDetailControl, dualButtonControl) = CreateChildren();
            dockPanel.Children.Add(viewsDetailControl);
            dockPanel.Children.Add(dualButtonControl);
            dockPanel.Children.Add(comboBox);
            Content = dockPanel;
        }

        private static (ComboBox comboBox, ViewsMasterDetailControl viewsDetailControl, DualButtonControl dualButtonControl) CreateChildren()
        {
            var comboBox = new AssemblyComboBox();
            DockPanel.SetDock(comboBox, Dock.Top);
            Binding binding = new()
            {
                Path = new PropertyPath(nameof(ComboBox.SelectedValue)),
                Source = comboBox
            };

            var viewsDetailControl = new ViewsMasterDetailControl { };
            BindingOperations.SetBinding(viewsDetailControl, ViewsMasterDetailControl.AssemblyProperty, binding);
            viewsDetailControl.DemoType = DemoType.ResourceDictionary;

            DualButtonControl dualButtonControl = new();
            dualButtonControl.Main = DemoType.UserControl;
            dualButtonControl.Alternate = DemoType.ResourceDictionary;
            dualButtonControl.ButtonToggle += DualButtonControl_ButtonToggle;
            DockPanel.SetDock(dualButtonControl, Dock.Top);

            return (comboBox, viewsDetailControl, dualButtonControl);

            void DualButtonControl_ButtonToggle(object sender, SwitchControl.ToggleEventArgs size)
            {
                comboBox.DemoType = Enum.Parse<DemoType>(size.Key.ToString());
            }
        }
    }
}