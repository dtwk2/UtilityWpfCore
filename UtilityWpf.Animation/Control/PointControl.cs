using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace UtilityWpf.Animation
{

    public class PointControl : Control
    {
        static PointControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointControl), new FrameworkPropertyMetadata(typeof(PointControl)));
        }

        public PointControl()
        {
        }

        private Path rctMovingObject;

        public override void OnApplyTemplate()
        {
            rctMovingObject = this.GetTemplateChild("PART_MovingObject") as Path;
            var myEllipseGeometry = new EllipseGeometry();
            myEllipseGeometry.Center = new Point(100, 50);
            myEllipseGeometry.RadiusX = 15;
            myEllipseGeometry.RadiusY = 15;
            rctMovingObject.Data = myEllipseGeometry;
        }

        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            set { SetValue(PointProperty, value); }
        }

        public static readonly DependencyProperty PointProperty = DependencyProperty.Register("Point", typeof(Point), typeof(PointControl), new PropertyMetadata(default(Point), BeatChanged));

        private static void BeatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointControl bc = d as PointControl;

            PointAnimation myPointAnimation = new PointAnimation
            {
                Duration = TimeSpan.FromSeconds(2),
                From = (Point)e.OldValue,
                To = (Point)e.NewValue
            };


            (bc.rctMovingObject?.Data as EllipseGeometry)?
                .BeginAnimation(EllipseGeometry.CenterProperty, myPointAnimation);
        }
    }

}
