using System.Collections;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.ViewModel.Livecharts;

namespace UtilityWpf.View.Livecharts
{
    public class DateTimeChart : Control
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(DateTimeChart), new PropertyMetadata(null, SeriesChanged));

        private static void SeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (d as DateTimeChart).Dispatcher.InvokeAsync(() =>
                  ((d as DateTimeChart).CartesianChart).Series = new TimeChartViewModel((dynamic)e.NewValue).SeriesCollection, System.Windows.Threading.DispatcherPriority.Background);
        }

        public override void OnApplyTemplate()
        {
            CartesianChart = this.GetTemplateChild("PART_Chart") as LiveCharts.Wpf.CartesianChart;
        }

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public LiveCharts.Wpf.CartesianChart CartesianChart { get; private set; }

        static DateTimeChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeChart), new FrameworkPropertyMetadata(typeof(DateTimeChart)));
            //SeriesProperty.OverrideMetadata(typeof(DateTimeChart), new FrameworkPropertyMetadata(typeof(DateTimeChart), FrameworkPropertyMetadataOptions.None, null, SeriesChanged));
        }

        public DateTimeChart()
        {
            //Uri resourceLocater = new Uri("/UtilityWpf.View;component/Themes/LiveChart/DateTimeChart.xaml", System.UriKind.Relative);
            //ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            //Style = resourceDictionary["DateTimeChart"] as Style;
        }
    }
}