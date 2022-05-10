using ReactiveUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityWpf.Adorners;
using UtilityWpf.Base;
using static UtilityWpf.Base.SettingsControl;
using Type = UtilityWpf.Adorners.Type;

namespace UtilityWpf.Demo.View
{
    public class SettingsAdorner : FrameworkElementAdorner
    {
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
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AdornedElement is Control control)
                control.Background = control.Background == Brushes.Red ? Brushes.PowderBlue : Brushes.Red;
            if (AdornedElement is Panel panel)
                panel.Background = panel.Background == Brushes.Red ? Brushes.PowderBlue : Brushes.Red;
        }
    }
}
