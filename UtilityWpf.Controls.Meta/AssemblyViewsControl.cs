using FreeSql;
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

            dockPanel.Children.Add(dualButtonControl);
            dockPanel.Children.Add(comboBox);
            dockPanel.Children.Add(viewsDetailControl);
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

            var dualButtonControl = new DualButtonControl
            {
                Main = DemoType.UserControl,
                Alternate = DemoType.ResourceDictionary
            };

            var first = DualButtonEntity.Select.First();
            if (first == null)
            {
                first = new DualButtonEntity { DemoType = DemoType.UserControl };
                first.Insert();
            }

            dualButtonControl.Value = dualButtonControl.KeyToValue(viewsDetailControl.DemoType = comboBox.DemoType = first.DemoType);

            dualButtonControl.ButtonToggle += DualButtonControl_ButtonToggle;
            DockPanel.SetDock(dualButtonControl, Dock.Top);

            return (comboBox, viewsDetailControl, dualButtonControl);

            void DualButtonControl_ButtonToggle(object sender, SwitchControl.ToggleEventArgs size)
            {
                var demoType = Enum.Parse<DemoType>(size.Key.ToString());
                viewsDetailControl.DemoType = comboBox.DemoType = first.DemoType = demoType;
                first.Update();
            }
        }
    }

    public class DualButtonEntity : BaseEntity<DualButtonEntity, Guid>
    {
        public DemoType DemoType { get; set; }
    }
}