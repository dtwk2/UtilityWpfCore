using System;
using System.Collections.Generic;
using System.Windows;
using UtilityWpf.Interactive.View.Controls;

namespace UtilityWpf.Chart
{
    public class OxyChartSelector : MasterDetailCheckView
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(string), typeof(OxyChartSelector), new PropertyMetadata(null, Changed));

        static OxyChartSelector()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChartSelector), new FrameworkPropertyMetadata(typeof(OxyChartSelector)));
        }

        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public OxyChartSelector()
        {
            var oxyChart = new OxyChart();

            Content = oxyChart;

            //this.Loaded += OxyChartSelector_Loaded;

            //   this.SelectChanges(nameof(Data))
            //       .ObserveOnDispatcher()
            //       .Subscribe(a =>
            //   {
            //       oxyChart.Data = a as IEnumerable;
            //   });

            //   this.SelectChanges(nameof(Id))
            //       .ObserveOnDispatcher()
            //.Subscribe(a =>
            //{
            //    oxyChart.Id = a.ToString();
            //});
        }

        protected override void SetCollection(object content, IReadOnlyCollection<object> objects)
        {
            if (content is OxyChart oview)
            {
                oview.ItemsSource = objects;
                oview.Data = Data;
                oview.Id = Id;
            }
            else throw new Exception(nameof(Content) + " needs to have property");
        }

        //private void OxyChartSelector_Loaded(object sender, RoutedEventArgs e)
        //{
        //    (Content as OxyChart).Data = Data;
        //    (Content as OxyChart).Id = Id;
        //}
    }
}