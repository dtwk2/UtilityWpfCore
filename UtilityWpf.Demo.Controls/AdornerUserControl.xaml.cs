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
using UtilityWpf.Utility;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class AdornerUserControl : UserControl
    {
        private readonly ControlColourer controlColourer;
        private readonly AdornerController adornerController;

        public AdornerUserControl()
        {
            InitializeComponent();
            TextCommand = new Command.RelayCommand(() => TextBlock1.Text += " New Text");
            Grid1.DataContext = this;
            controlColourer = new(this);
            adornerController = new(Square3Grid, new PackIcon
            {
                Height = 24,
                Width = 24,
                Kind = PackIconKind.Gear,
                Margin = new Thickness(4),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            });
        }

        public ICommand TextCommand { get; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock1.Text.Length >= 9)
                TextBlock1.Text = TextBlock1.Text.Remove(TextBlock1.Text.Length - 9);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            foreach (UIElement ui in canvas.Children)
                layer.Add(new ResizeAdorner(ui));

            layer.Add(new VerticalAxisAdorner(this.Grid));
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            Adorner[] toRemoveArray;

            foreach (UIElement ui in canvas.Children)
            {
                toRemoveArray = layer.GetAdorners(ui);
                if (toRemoveArray != null)
                    foreach (Adorner a in toRemoveArray)
                    {
                        layer.Remove(a);
                    }
            }

            toRemoveArray = layer.GetAdorners(Grid);
            if (toRemoveArray != null)
                foreach (Adorner a in toRemoveArray)
                {
                    layer.Remove(a);
                }
        }

        private bool flag;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                controlColourer.Remove();
                adornerController.Remove();
            }
            else
            {
                controlColourer.Apply();
                adornerController.Apply();
            }
            flag = !flag;
        }

    }

    class AdornerController
    {
        private readonly DependencyObject adornedElement;
        private readonly DependencyObject dependencyObject;

        public AdornerController(DependencyObject adornedElement, DependencyObject dependencyObject)
        {
            this.adornedElement = adornedElement;
            this.dependencyObject = dependencyObject;
        }

        public void Apply()
        {
            adornedElement.SetValue(AdornerEx.IsEnabledProperty, true);
            var adorners = AdornerEx.GetAdorners(adornedElement);
            if (adorners.IndexOf(dependencyObject) == -1)
                adorners.Add(dependencyObject);
        }

        public void Remove()
        {
            adornedElement.SetValue(AdornerEx.IsEnabledProperty, false);
        }
    }

    class ControlColourer
    {
        private readonly DependencyObject dependencyObject;
        Dictionary<Guid, Brush> originalBrushes = new();
        Dictionary<Guid, Brush> tempBrushes = new();

        public ControlColourer(DependencyObject dependencyObject)
        {
            this.dependencyObject = dependencyObject;
        }

        public void Apply()
        {
            foreach (var child in dependencyObject.FindChildren<Control>())
            {
                var background = child.Background;
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
                foreach (var child in dependencyObject.FindChildren<Control>())
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
