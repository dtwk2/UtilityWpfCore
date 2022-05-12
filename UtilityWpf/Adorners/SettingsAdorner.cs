using RandomColorGenerator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityWpf.Attached;
using UtilityWpf.Base;
using UtilityWpf.Utility;
using static UtilityWpf.Base.SettingsControl;

namespace UtilityWpf.Adorners
{
    public class SettingsAdorner : FrameworkElementAdorner
    {
        private ControlColourer? controlColourer;
        private IDisposable disposable;

        public SettingsAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        public override void SetAdornedElement(DependencyObject adorner, FrameworkElement? adornedElement)
        {
            if (adorner is SettingsControl settingsControl)
            {
                if (adornedElement == null)
                {
                    disposable?.Dispose();
                    return;
                }
                else
                {
                    adornedElement.SetValue(Type.ShowDataContextProperty, true);
                    disposable = settingsControl
                                   .WhenAnyValue(a => a.SelectedDock)
                                   .Subscribe(dock =>
                                   {
                                       adornedElement.SetValue(Text.PositionProperty, dock);
                                   });

                    settingsControl.Checked += SettingsControl_Checked;
                }
            }

            void SettingsControl_Checked(object sender, CheckedEventArgs e)
            {
                switch (e.CheckedType)
                {
                    case CheckedType.DataContext:
                        AdornedElement.SetValue(Type.ShowDataContextProperty, e.IsChecked);
                        break;

                    case CheckedType.Dimensions:
                        AdornedElement.SetValue(Type.ShowDimensionsProperty, e.IsChecked);
                        break;

                    case CheckedType.HighlightArea:
                        controlColourer ??= new(adornedElement);
                        //AdornedElement.SetValue(Type.HighlightColourProperty, e.IsChecked);
                        if (e.IsChecked)
                        {
                            controlColourer.Apply();
                        }
                        else
                        {
                            controlColourer.Remove();
                        }
                        break;
                }
            }
        }

        internal class ControlColourer
        {
            private readonly DependencyObject dependencyObject;
            private readonly Dictionary<Guid, Brush> originalBrushes = new();
            private readonly Dictionary<Guid, Brush> tempBrushes = new();

            public ControlColourer(DependencyObject dependencyObject)
            {
                this.dependencyObject = dependencyObject;
            }

            public void Apply()
            {
                foreach (Control child in dependencyObject.FindChildren<Control>())
                {
                    Brush background = child.Background;
                    Guid guid = (Guid)child.GetValue(Ex.KeyProperty);
                    child.SetValue(Ex.KeyProperty, guid);
                    child.Background = tempBrushes.ContainsKey(guid) ? tempBrushes[guid] : RandomColor.GetColor(ColorScheme.Random, Luminosity.Light).ToMediaBrush();
                    originalBrushes[guid] = background;
                    tempBrushes[guid] = child.Background;
                }
            }

            public void Remove()
            {
                if (tempBrushes.Any())
                {
                    foreach (Control child in dependencyObject.FindChildren<Control>())
                    {
                        Guid? guid = (Guid?)child.GetValue(Ex.KeyProperty);
                        if (guid.HasValue && originalBrushes.ContainsKey(guid.Value))
                        {
                            child.Background = originalBrushes[guid.Value];
                        }
                        else
                        {
                            // child's been removed
                        }
                    }
                }
            }
        }
    }
}