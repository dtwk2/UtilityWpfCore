using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.DemoAnimation
{
    /// <summary>
    /// Interaction logic for MaterialDesignUserControl.xaml
    /// </summary>
    public partial class MaterialDesignUserControl : UserControl, INotifyPropertyChanged
    {
        private readonly Dictionary<string, PackIconKind> values;

        public MaterialDesignUserControl()
        {
            InitializeComponent();

            this.DataContext = this;
            this.TextBox1.TextChanged += TextBox1_TextChanged;
            values = Enum.GetValues(typeof(PackIconKind)).Cast<PackIconKind>().GroupBy(a => a.ToString()).ToDictionary(a => a.Key.ToLowerInvariant(), a => a.First());
            PackIconChange(values.First().Value);
        }

        public string Text { get; set; }

        public PackIconKind Value { get; set; }

        public bool Value2 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = TextBox1.Text;
            PackIconKind value = default;
            while (value == default && Text.Length > 1)
            {
                value = values.GetValueOrDefault(Text.ToLowerInvariant());
                Text = Text.Remove(0, Text.Length - 1);
            }

            PackIconChange(value);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PackIconChange(((sender as Button)?.Content as PackIcon)?.Kind);
        }

        private void PackIconChange(PackIconKind? value)
        {

            if (value.HasValue == false || Value == value)
                return;
            Value = value.Value;

            // TransitionContent.Visibility = TransitionContent.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            TransitionBox.Content = new PackIcon
            {
                Width = 70,
                Height = 70,
                Margin = new Thickness(4),
                Kind = Value
            };

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value2"));
        }
    }
}
