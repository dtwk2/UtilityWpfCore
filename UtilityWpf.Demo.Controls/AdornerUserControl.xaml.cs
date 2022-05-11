using MaterialDesignThemes.Wpf;
using RandomColorGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UtilityWpf.Adorners;
using UtilityWpf.Attached;
using UtilityWpf.Base;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Utility;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class AdornerUserControl : UserControl
    {
        private readonly AdornerController adornerController;
        private readonly ControlColourer controlColourer;
        private bool flag;

        public AdornerUserControl()
        {
            InitializeComponent();
            TextCommand = new Command.RelayCommand(() => TextBlock1.Text += " New Text");
            Grid1.DataContext = this;
            controlColourer = new(this);
            //adornerController = new(Square3Grid);
            GearGrid.SetValue(AdornerEx.AdornerProperty, new CustomFrameworkElementAdorner(GearGrid));
            Square3Grid.SetValue(DataContextProperty, new Characters());
            Square3Grid.SetValue(AdornerEx.AdornerProperty, new SettingsAdorner(Square3Grid));
            Square3Grid.SetValue(AdornerEx.IsEnabledProperty, true);
            //Square3Grid.AddIfMissingAdorner(new SettingsControl());
        }

        public ICommand TextCommand { get; }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(canvas);
            foreach (UIElement ui in canvas.Children)
            {
                layer.Add(new ResizeAdorner(ui));
            }

            layer.Add(new VerticalAxisAdorner(Grid));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                controlColourer.Remove();
                adornerController?.Hide();
            }
            else
            {
                controlColourer.Apply();
                adornerController?.Apply();
            }
            flag = !flag;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock1.Text.Length >= 9)
            {
                TextBlock1.Text = TextBlock1.Text.Remove(TextBlock1.Text.Length - 9);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement ui in canvas.Children)
            {
                ui.RemoveAdorners();
            }

            Grid.RemoveAdorners();
        }
    }

    public class CustomFrameworkElementAdorner : FrameworkElementAdorner
    {
        public CustomFrameworkElementAdorner(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        public override void SetAdornedElement(DependencyObject adorner, FrameworkElement? adornedElement)
        {
            if (adorner is Button button)
            {
                if (adornedElement == null)
                {
                    button.Click -= Button_Click;
                }
                else
                {
                    button.Click += Button_Click;
                }
            }

            void Button_Click(object sender, RoutedEventArgs e)
            {
                if (AdornedElement is Control control)
                {
                    control.Background = control.Background == Brushes.Red ? Brushes.PowderBlue : Brushes.Red;
                }

                if (AdornedElement is Panel panel)
                {
                    panel.Background = panel.Background == Brushes.Red ? Brushes.PowderBlue : Brushes.Red;
                }
            }
        }
    }

    internal class AdornerController
    {
        private readonly UIElement adornedElement;
        private readonly DependencyObject adorner;

        public AdornerController(UIElement adornedElement, DependencyObject? dependencyObject = null)
        {
            this.adornedElement = adornedElement;
            adorner = dependencyObject ?? new SettingsControl();
        }

        public void Apply()
        {
            adornedElement.AddIfMissingAdorner(adorner);
            adornedElement.SetValue(AdornerEx.IsEnabledProperty, true);
        }

        public void Hide()
        {
            adornedElement.SetValue(AdornerEx.IsEnabledProperty, false);
        }

        public void Remove()
        {
            adornedElement.RemoveAdorners();
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
                Guid guid = (Guid?)child.GetValue(Ex.KeyProperty) ?? Guid.NewGuid();
                child.SetValue(Ex.KeyProperty, guid);
                child.Background = tempBrushes.ContainsKey(guid) ? tempBrushes[guid] : RandomColor.GetColor(ColorScheme.Random, Luminosity.Bright).ToMediaBrush();
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