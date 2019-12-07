using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    // for displaying key value pairs
    public class DictionaryControl : ItemsControl
    {
        static DictionaryControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DictionaryControl), new FrameworkPropertyMetadata(typeof(DictionaryControl)));
            //Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/DictionaryControl.xaml", System.UriKind.Relative);
            //ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            //Style = resourceDictionary["DictionaryStyle"] as Style;
        }

        public DictionaryControl()
        {
        }
    }
}