using System;
using System.Collections;
using System.Reactive.Linq;
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

            this.SelectChanges(nameof(Data))
                .ObserveOnDispatcher()
                .Subscribe(a =>
            {
                oxyChart.Data = a as IEnumerable;
            });

            this.SelectChanges(nameof(Id))      
                .ObserveOnDispatcher()
         .Subscribe(a =>
         {
             oxyChart.Id = a.ToString();
         });
        }

        private void OxyChartSelector_Loaded(object sender, RoutedEventArgs e)
        {
            (DetailView as OxyChart).Data = Data;
            (DetailView as OxyChart).Id = Id;
        }
    }



}
