using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Demo.Adorners.Infrastructure;
using UtilityWpf.Controls.Meta;
using UtilityWpf.Demo.Data.Factory;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        private readonly AdornerController adornerController;
        private readonly ControlColourer controlColourer;
        private bool flag;

        public SettingsUserControl()
        {
            InitializeComponent();

            controlColourer = new(this);
            adornerController = new(this);

            Square3Grid.DataContext = DataContexts.Random;
            SettingsAdorner.AddTo(Square3Grid);

            GearGrid.SetValue(AdornerEx.AdornerProperty, new CustomFrameworkElementAdorner(GearGrid));
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
    }
}
