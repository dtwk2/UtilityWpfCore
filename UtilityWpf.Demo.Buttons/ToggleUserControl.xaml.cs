using System.ComponentModel;
using System.Windows.Controls;

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

        }

    }
}