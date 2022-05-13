﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for AdornerUser.xaml
    /// </summary>
    public partial class AdornerUserControl : UserControl
    {


        public AdornerUserControl()
        {
            InitializeComponent();
            TextCommand = new UtilityWpf.Command.RelayCommand(() => TextBlock1.Text += " New Text");
            Grid1.DataContext = this;

            //adornerController = new(Square3Grid);


        }

        //Square3Grid.DataContext = new Characters();
        //var adorner = new SettingsAdorner(Square3Grid);
        //Square3Grid.SetValue(AdornerEx.AdornerProperty, adorner);

        public ICommand TextCommand { get; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock1.Text.Length >= 9)
            {
                TextBlock1.Text = TextBlock1.Text.Remove(TextBlock1.Text.Length - 9);
            }
        }
    }
}