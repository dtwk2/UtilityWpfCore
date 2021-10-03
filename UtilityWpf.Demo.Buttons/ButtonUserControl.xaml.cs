﻿using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityWpf.Controls.Buttons;

namespace UtilityWpf.Demo.Buttons
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