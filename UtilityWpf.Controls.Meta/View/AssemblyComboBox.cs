using FreeSql;
using System;
using System.Windows;
using UtilityInterface.NonGeneric;
using UtilityWpf.Controls.Buttons;
using UtilityWpf.Model;
using Evan.Wpf;
using UtilityWpf.Controls.Meta.ViewModel;

namespace UtilityWpf.Controls.Meta
{
    internal class AssemblyComboBox : CheckBoxesComboControl
    {
        public static readonly DependencyProperty DemoTypeProperty = DependencyHelper.Register();

        public AssemblyComboBox()
        {
            //SelectedIndex = 0;
            FontWeight = FontWeights.DemiBold;
            //FontSize = 14;
            Margin = new Thickness(4);
            //Width = (this.Parent as FrameworkElement)?.ActualWidth ?? 1000;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Height = 80;
            DisplayMemberPath = nameof(AssemblyKeyValue.Key);
            IsCheckedPath = nameof(Model.KeyValue.IsChecked);
            IsSelectedPath = nameof(Model.KeyValue.IsSelected);
            SelectedValuePath = nameof(AssemblyKeyValue.Value);

            var dis = AssemblyComboBoxViewModelMapper.Connect(this, new AssemblyComboBoxViewModel());
            this.Unloaded += (s, e) => dis.Dispose();
        }
     

        public DemoType DemoType
        {
            get => (DemoType)GetValue(DemoTypeProperty);
            set => SetValue(DemoTypeProperty, value);
        }
    }
}