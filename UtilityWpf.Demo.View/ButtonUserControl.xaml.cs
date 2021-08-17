using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityWpf.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for PathButtonUserControl.xaml
    /// </summary>
    public partial class ButtonUserControl : UserControl
    {
        private readonly TypeConverter converter;

        public ButtonUserControl()
        {
            InitializeComponent();

            this.PathTextBox.Text = PathButton.InitialData;
            converter = TypeDescriptor.GetConverter(typeof(Geometry));
            this.PathTextBox.TextChanged += PathTextBox_TextChanged;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ErrorTextBlock.Text = string.Empty;
            try
            {
                PathButton.PathData = (Geometry)converter.ConvertFrom(this.PathTextBox.Text);
            }
            catch (Exception ex)
            {
                this.ErrorTextBlock.Text = ex.Message;
            }
        }
    }
}