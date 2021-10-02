using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UtilityWpf.Adorners;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class AdornerUserControl : UserControl
    {
        public AdornerUserControl()
        {
            InitializeComponent();
            TextCommand = new Command.RelayCommand(() => TextBlock1.Text += " New Text");
            Grid1.DataContext = this;
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
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);

            foreach (UIElement ui in canvas.Children)
            {
                Adorner[] toRemoveArray = layer.GetAdorners(ui);
                if (toRemoveArray != null)
                    foreach (Adorner a in toRemoveArray)
                    {
                        layer.Remove(a);
                    }
            }
        }

        private void Add_2_Click(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            foreach (UIElement ui in canvas.Children)
                layer.Add(new ListAdorner(ui));
        }
    }


    public class ListAdorner : Adorner
    {
        public ListAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }
    }
}
