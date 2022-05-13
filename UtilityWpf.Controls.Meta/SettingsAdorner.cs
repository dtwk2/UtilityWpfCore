using HandyControl.Controls;
using RandomColorGenerator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.WPF.Adorners;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Attached;
using Utility.WPF.Helper;
using UtilityWpf.Base;
using static UtilityWpf.Base.SettingsControl;
using Type = Utility.WPF.Adorners.Type;

namespace UtilityWpf.Controls.Meta
{
    public class SettingsAdorner : FrameworkElementAdorner
    {
        private ControlColourer? controlColourer;
        private IDisposable disposable;

        private SettingsAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        public static SettingsAdorner AddTo(FrameworkElement adornedElement)
        {
            var settingsAdorner = new SettingsAdorner(adornedElement);
            adornedElement.SetValue(AdornerEx.AdornerProperty, settingsAdorner);
            adornedElement.SetValue(AdornerEx.IsEnabledProperty, true);
            adornedElement.AddIfMissingAdorner(new SettingsControl());
            return settingsAdorner;
        }

        public override void SetAdornedElement(DependencyObject adorner, FrameworkElement? adornedElement)
        {
            if (adorner is SettingsControl settingsControl)
            {
                if (adornedElement == null)
                {
                    settingsControl.Checked -= SettingsControl_Checked;
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
                foreach (FrameworkElement child in dependencyObject.FindChildren<FrameworkElement>().Prepend(dependencyObject))
                {
                    Guid guid = (Guid?)child.GetValue(Ex.KeyProperty) ?? Guid.NewGuid();
                    Brush? background = default;
                    if (child is Control control)
                    {
                        background = ApplyBrush(guid, () => control.Background, b => control.Background = b);
                    }
                    else if (child is Panel panel)
                    {
                        background = ApplyBrush(guid, () => panel.Background, b => panel.Background = b);
                    }
                    else
                    {
                        return;
                    }

                    child.SetValue(Ex.KeyProperty, guid);
                    originalBrushes[guid] = background;
                }

                Brush ApplyBrush(Guid guid, Func<Brush> func, Action<Brush> action)
                {
                    Brush background = func();
                    action(tempBrushes[guid] = Brush(guid));
                    return background;

                    Brush Brush(Guid guid)
                    {
                        return tempBrushes.ContainsKey(guid) ?
                            tempBrushes[guid] :
                            RandomColor.GetColor(ColorScheme.Random, Luminosity.Light)
                                       .ToMediaBrush()
                                       .WithOpacity(0.5);
                    }
                }

            }

            public void Remove()
            {
                if (tempBrushes.Any())
                {
                    foreach (FrameworkElement child in dependencyObject.FindChildren<FrameworkElement>().Prepend(dependencyObject))
                    {
                        Guid? guid = (Guid?)child.GetValue(Ex.KeyProperty);
                        if (guid.HasValue && originalBrushes.ContainsKey(guid.Value))
                        {
                            if (child is Control control)
                            {
                                control.Background = originalBrushes[guid.Value];
                            }
                            else if (child is Panel panel)
                            {
                                panel.Background = originalBrushes[guid.Value];
                            }
                            else
                            {
                                throw new Exception("REG34 hdfgghfd");
                            }
                        }
                        else
                        {
                            // child's probably been removed
                        }
                    }
                }
            }
        }
    }
}