using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitsNet.Units;
using _ViewModel = Utility.ViewModel.ViewModel;
namespace UtilityWpf.Demo.Forms.ViewModel
{
    public class MeasurementsViewModel : _ViewModel
    {
        private LengthUnit? unit;

        public MeasurementsViewModel(string header, IReadOnlyCollection<MeasurementViewModel> collection) : base(header)
        {
            Collection = new(collection);
            Intialise();
        }

        public override ObservableCollection<MeasurementViewModel> Collection { get; } /*= new MeasurementViewModel[] { new MeasurementViewModel { Header = "asd", Value = 0 } };*/

        public LengthUnit? Unit { get => unit; set => unit = value; }
    }

    public class MeasurementViewModel
    {
        public string? Header { get; init; }
        public double Value { get; set; }
    }
}