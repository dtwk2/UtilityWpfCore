using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class AdornerUser : UserControl
    {
        public AdornerUser()
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
    }
}