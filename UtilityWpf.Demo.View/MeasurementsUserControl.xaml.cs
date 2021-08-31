using System.Collections;
using System.Windows.Controls;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for MeasurementsUserControl.xaml
    /// </summary>
    public partial class MeasurementsUserControl : UserControl
    {
        public MeasurementsUserControl()
        {
            InitializeComponent();
        }
    }

    public class ViewModel
    {
        public IEnumerable Collection { get; } = new[] { new MeasurementViewModel { Header = "Height" }, new MeasurementViewModel { Header = "Width" } };
    }


    public class MeasurementViewModel
    {
        public string Header { get; init; }
        public double Value { get; init; }
    }
}
