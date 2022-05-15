using FreeSql;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Persist;
using UtilityWpf.Controls.Buttons;

namespace UtilityWpf.Controls.Meta
{
    public class AssemblyViewsControl : ContentControl
    {
        public AssemblyViewsControl()
        {
            var dockPanel = new DockPanel();

            foreach (var child in CreateChildren())
            {
                dockPanel.Children.Add(child);
            }

            Content = dockPanel;
        }

        private static IEnumerable<Control> CreateChildren()
        {
            var viewModel = new ViewModel();
            var b = CreateDualButtonControl(viewModel.DemoType);
            Connector.Connect(b, viewModel);
            yield return b;

            var c = CreateAssemblyComboBox(viewModel.DemoType);
            Connector.Connect(c, viewModel);
            yield return c;

            var v = CreateViewsMasterDetailControl(c);
            Connector.Connect(v, viewModel);
            yield return v;
        

            static DualButtonControl CreateDualButtonControl(DemoType demoType)
            {
                var dualButtonControl = new DualButtonControl
                {
                    Main = DemoType.UserControl,
                    Alternate = DemoType.ResourceDictionary,
                    Orientation = Orientation.Horizontal,
                    Value = DualButtonControl.KeyToValue(demoType, DemoType.UserControl, DemoType.ResourceDictionary)
                };
                DockPanel.SetDock(dualButtonControl, Dock.Top);
                return dualButtonControl;
            }

            static AssemblyComboBox CreateAssemblyComboBox(DemoType demoType)
            {
                var comboBox = new AssemblyComboBox(demoType);
                DockPanel.SetDock(comboBox, Dock.Top);
                return comboBox;
            }

            static ViewsMasterDetailControl CreateViewsMasterDetailControl(AssemblyComboBox comboBox)
            {
                var viewsDetailControl = new ViewsMasterDetailControl { };
                BindingOperations.SetBinding(
                    viewsDetailControl,
                    ViewsMasterDetailControl.AssemblyProperty,
                    CreateBinding(comboBox));
                return viewsDetailControl;

                static Binding CreateBinding(AssemblyComboBox comboBox)
                {
                    return new()
                    {
                        Path = new PropertyPath(nameof(ComboBox.SelectedValue)),
                        Source = comboBox
                    };
                }
            }
        }

        class Connector
        {
            public static void Connect(DualButtonControl dualButtonControl, ViewModel viewModel)
            {
                dualButtonControl
                    .Toggles()
                    .Select(size => Enum.Parse<DemoType>(size.Key.ToString()))
                    .Subscribe(demoType => viewModel.DemoType = demoType);
            }

            public static void Connect(AssemblyComboBox comboBox, ViewModel viewModel)
            {
                viewModel
                    .WhenAnyValue(a => a.DemoType)
                    .Subscribe(a => comboBox.DemoType = a);
            }

            public static void Connect(ViewsMasterDetailControl detailControl, ViewModel viewModel)
            {
                viewModel
                    .WhenAnyValue(a => a.DemoType)
                    .Subscribe(a => detailControl.DemoType = a);
            }
        }

        class ViewModel : ReactiveObject
        {
            private DualButtonEntity? first;
            private bool isInitialised = false;

            public DemoType DemoType
            {
                get
                {
                    if (!isInitialised)
                    {
                        FreeSqlFactory.InitialiseSQLite();
                        isInitialised = true;
                    }
                    first ??= DualButtonEntity.Select.First();

                    if (first == null)
                    {
                        first = new DualButtonEntity { DemoType = Meta.DemoType.UserControl };
                        first.InsertAsync();
                    }

                    return first.DemoType;
                }
                set
                {
                    (first ??= DualButtonEntity.Select.First()).DemoType = value;
                    first.UpdateAsync();
                    this.RaisePropertyChanged();
                }
            }
            class DualButtonEntity : BaseEntity<DualButtonEntity, Guid>
            {
                public DemoType DemoType { get; set; }
            }
        }
    }
}