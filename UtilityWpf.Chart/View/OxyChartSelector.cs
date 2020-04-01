using System.Collections;
using System.Windows;
using UtilityWpf.View;

namespace UtilityWpf.Chart
{
    public class OxyChartSelector : MasterDetailCheckView
    {
        static OxyChartSelector()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChartSelector), new FrameworkPropertyMetadata(typeof(OxyChartSelector)));
        }

        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable), typeof(OxyChartSelector), new PropertyMetadata(null, Changed));

        public OxyChartSelector()
        {
            var oxyChart = new OxyChart();

            DetailView = oxyChart;

            this.Loaded += OxyChartSelector_Loaded;

        }

        private void OxyChartSelector_Loaded(object sender, RoutedEventArgs e)
        {
            (DetailView as OxyChart).Data = Data;
            (DetailView as OxyChart).IdProperty = Id;
        }
    }



}
