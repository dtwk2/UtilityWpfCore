using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityWpf.DemoAnimation
{
    /// <summary>
    /// Interaction logic for PointAnimationUserControl.xaml
    /// </summary>
    public partial class PointAnimationUserControl : UserControl
    {
        Path myPath = null; EllipseGeometry myEllipseGeometry = null; Geometry myEllipseGeometry2 = null;
        public PointAnimationUserControl()
        {
            InitializeComponent();
            myEllipseGeometry = new EllipseGeometry();
            myEllipseGeometry.Center = new Point(200, 100);
            myEllipseGeometry.RadiusX = 15;
            myEllipseGeometry.RadiusY = 15;


            myPath = new Path();
            myPath.Fill = Brushes.Blue;
            myPath.Margin = new Thickness(15);
            myPath.Data = myEllipseGeometry;
            Canvas1.Children.Add(myPath);



        }


        public void PointAnimationExample()
        {

            PointAnimation myPointAnimation = new PointAnimation
            {
                Duration = TimeSpan.FromSeconds(2),
                From = new Point(200, 100),
                To = new Point(450, 250)
            };


            myEllipseGeometry.BeginAnimation(EllipseGeometry.CenterProperty, myPointAnimation);

        }
        Random r = new Random();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PointAnimationExample();
            int amt = 40;
            PointControl1.Point = r.NextDouble() > 0.75 ?
                new Point(PointControl1.Point.X + amt, PointControl1.Point.Y + amt) :
                 r.NextDouble() > 0.5 ?
                new Point(PointControl1.Point.X + amt, PointControl1.Point.Y - amt) :
                      r.NextDouble() > 0.25 ?
                new Point(PointControl1.Point.X - amt, PointControl1.Point.Y + amt) :
                new Point(PointControl1.Point.X - amt, PointControl1.Point.Y - amt);


        }
    }

}
