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

        public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register("PathData", typeof(System.Windows.Media.Geometry), typeof(PathButton), new PropertyMetadata());

        public System.Windows.Media.Geometry PathData
        {
            get { return (System.Windows.Media.Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        //private static System.Windows.Media.Geometry pathData;

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(System.Windows.Media.Brush), typeof(PathButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));

        public System.Windows.Media.Brush HoverBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            //(this.GetTemplateChild("ButtonPath") as Path).Data = PathData;

            base.OnApplyTemplate();
        }

        static PathButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathButton), new FrameworkPropertyMetadata(typeof(PathButton)));
        }

        public PathButton()
        {
            string sData = "M 250,40 L200,20 L200,60 Z";
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Windows.Media.Geometry));
            PathData = (System.Windows.Media.Geometry)converter.ConvertFrom(sData);

            //Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/PathButton.xaml", System.UriKind.Relative);
            //ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            //Style = resourceDictionary["PathButtonStyle"] as Style;

            //if (PathData == null)
            //{
            //    PathData = pathData;
            //}
        }
    }
}