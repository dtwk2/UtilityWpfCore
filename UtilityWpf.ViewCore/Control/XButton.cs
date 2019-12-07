using System;
using System.Windows.Media;

namespace UtilityWpf.View
{
    public class XButton : PathButton
    {
        public XButton()
        {
            var myResourceDictionary = new System.Windows.ResourceDictionary();
            myResourceDictionary.Source = new Uri("/UtilityWpf.ViewCore;component/Themes/Geometry.xaml", UriKind.RelativeOrAbsolute);
            var path = myResourceDictionary["Cross"];
            PathData = (System.Windows.Media.Geometry)path;

            HoverBackground = new System.Windows.Media.SolidColorBrush(Colors.IndianRed);
        }
    }
}