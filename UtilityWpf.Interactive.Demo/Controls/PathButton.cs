using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilityWpf.View
{
    public class PathButton : Button
    {
        public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register("PathData", typeof(System.Windows.Media.Geometry), typeof(PathButton), new PropertyMetadata());
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(System.Windows.Media.Brush), typeof(PathButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        static PathButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathButton), new FrameworkPropertyMetadata(typeof(PathButton)));
        }

        public PathButton()
        {
            string sData = "M 250,40 L200,20 L200,60 Z";
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Geometry));
            PathData = (System.Windows.Media.Geometry)converter.ConvertFrom(sData);
        }
    }
}