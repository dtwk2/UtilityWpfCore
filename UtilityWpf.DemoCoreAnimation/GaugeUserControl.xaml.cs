using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reactive;
using System.Reactive.Linq;


namespace UtilityWpf.DemoAnimation
{
    /// <summary>
    /// Interaction logic for GaugeUserControl.xaml
    /// </summary>
    public partial class GaugeUserControl : UserControl
    {
        public GaugeUserControl()
        {
            InitializeComponent();

            Observable
               .Interval(TimeSpan.FromSeconds(0.1))
               .ObserveOnDispatcher()
               .Subscribe(a =>
           {

               circProg.Value = a % 90;
           });
        }
    }

    public class DoubleToPctConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "";
            double d = (double)value;
            result = string.Format("{0}%", d);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// <a href="https://wpf.2000things.com/tag/circular-progress/"></a>
    /// </summary>
    public class CircularProgress : Shape
    {
        static CircularProgress()
        {
            Brush myGreenBrush = new SolidColorBrush(Colors.CadetBlue);
            myGreenBrush.Freeze();

            StrokeProperty.OverrideMetadata(                typeof(CircularProgress),                new FrameworkPropertyMetadata(myGreenBrush));

            FillProperty.OverrideMetadata(                typeof(CircularProgress),                new FrameworkPropertyMetadata(Brushes.Transparent));

            StrokeThicknessProperty.OverrideMetadata(                typeof(CircularProgress),                new FrameworkPropertyMetadata(10.0));
        }

        // Value (0-100)
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // DependencyProperty - Value (0 - 100)
        private static FrameworkPropertyMetadata valueMetadata =
                new FrameworkPropertyMetadata(
                    0.0,     // Default value
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,    // Property changed callback
                    new CoerceValueCallback(CoerceValue));   // Coerce value callback

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CircularProgress), valueMetadata);

        private static object CoerceValue(DependencyObject depObj, object baseVal)
        {
            double val = (double)baseVal;
            val = Math.Min(val, 100.0);
            val = Math.Max(val, 0.0);
            return val;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                double startAngle = 235.0;
                double endAngle = startAngle - ((Value / 100.0) * 270.0);

                double maxWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                double maxHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);

                double xStart = maxWidth / 2.0 * Math.Cos(startAngle * Math.PI / 180.0);
                double yStart = maxHeight / 2.0 * Math.Sin(startAngle * Math.PI / 180.0);

                double xEnd = maxWidth / 2.0 * Math.Cos(endAngle * Math.PI / 180.0);
                double yEnd = maxHeight / 2.0 * Math.Sin(endAngle * Math.PI / 180.0);

                StreamGeometry geom = new StreamGeometry();
                using (StreamGeometryContext ctx = geom.Open())
                {
                    ctx.BeginFigure(
                        new Point((RenderSize.Width / 2.0) + xStart,
                                  (RenderSize.Height / 2.0) - yStart),
                        true,   // Filled
                        false);  // Closed
                    ctx.ArcTo(
                        new Point((RenderSize.Width / 2.0) + xEnd,
                                  (RenderSize.Height / 2.0) - yEnd),
                        new Size(maxWidth / 2.0, maxHeight / 2),
                        0.0,     // rotationAngle
                        (startAngle - endAngle) > 180,   // greater than 180 deg?
                        SweepDirection.Clockwise,
                        true,    // isStroked
                        false);
                    //    ctx.LineTo(new Point((RenderSize.Width / 2.0), (RenderSize.Height / 2.0)), true, true);
                }

                return geom;
            }
        }
    }
}
