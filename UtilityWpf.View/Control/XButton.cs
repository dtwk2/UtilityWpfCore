using System;
using System.Windows.Media;

namespace UtilityWpf.View
{
    public class XButton : PathButton
    {
        public XButton()
        {
            PathData = ResourceHelper.FindRelativeResource<Geometry>("Themes/Geometry.xaml", "Cross");
            HoverBackground = new System.Windows.Media.SolidColorBrush(Colors.IndianRed);
        }
    }
}