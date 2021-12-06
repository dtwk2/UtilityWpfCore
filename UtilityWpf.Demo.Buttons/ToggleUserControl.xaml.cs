using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityWpf.Controls.Buttons;

namespace UtilityWpf.Demo.Buttons
{
    /// <summary>
    /// Interaction logic for ToggleUserControl.xaml
    /// </summary>
    public partial class ToggleUserControl : UserControl
    {
        private readonly TypeConverter converter;

        public ToggleUserControl()
        {
            InitializeComponent();

            this.PathTextBox.Text = GeometryButton.InitialData;
            converter = TypeDescriptor.GetConverter(typeof(Geometry));
            this.PathTextBox.TextChanged += PathTextBox_TextChanged;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ErrorTextBlock.Text = string.Empty;
            try
            {
                PathButton.Data = (Geometry)converter.ConvertFrom(this.PathTextBox.Text);
            }
            catch (Exception ex)
            {
                this.ErrorTextBlock.Text = ex.Message;
            }
        }
    }
}
