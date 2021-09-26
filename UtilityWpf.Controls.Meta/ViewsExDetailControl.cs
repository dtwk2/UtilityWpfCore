using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Model;

namespace UtilityWpf.Controls.Meta
{
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
}


