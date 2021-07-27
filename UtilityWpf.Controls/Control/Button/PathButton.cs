using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilityWpf.Controls
{
    public class PathButton : Button
    {
        public const string InitialData = "M 250,40 L200,20 L200,60 Z";

        public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register("PathData", typeof(Geometry), typeof(PathButton), new PropertyMetadata());
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(PathButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));

        static PathButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathButton), new FrameworkPropertyMetadata(typeof(PathButton)));
        }

        public PathButton()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Geometry));
            PathData = (Geometry)converter.ConvertFrom(InitialData);
        }
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

    }
}