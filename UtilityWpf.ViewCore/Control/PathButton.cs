using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilityWpf.View
{
    //[TemplatePart(Name = PART_Pathx, Type = typeof(Path))]
    public class PathButton : Button
    {
        //private const string PART_Pathx = "PART_Pathx";

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("PathData", typeof(System.Windows.Media.Geometry), typeof(PathButton), new PropertyMetadata(pathData));

        public System.Windows.Media.Geometry PathData
        {
            get { return (System.Windows.Media.Geometry)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        private static System.Windows.Media.Geometry pathData;
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(System.Windows.Media.Brush), typeof(PathButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));

        public System.Windows.Media.Brush HoverBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        static PathButton()
        {
            //  Set the style key, so that our control template is used.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathButton), new FrameworkPropertyMetadata(typeof(PathButton)));

            Path path = new Path();
            string sData = "M 250,40 L200,20 L200,60 Z";
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Windows.Media.Geometry));
            pathData = (System.Windows.Media.Geometry)converter.ConvertFrom(sData);
        }

        public PathButton()
        {
            //Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/PathButton.xaml", System.UriKind.Relative);
            //ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            //Style = resourceDictionary["PathButtonStyle"] as Style;

            if (PathData == null)
            {
                PathData = pathData;
            }
        }
    }
}