using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls.Master
{
    using fac = DependencyPropertyFactory<OrientationListBox>;

    //TODO move to UtilityWpf
    public class OrientationListBox : ListBox, IOrientation
    {
        public static readonly DependencyProperty OrientationProperty = fac.Register<Orientation>();

        static OrientationListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OrientationListBox), new FrameworkPropertyMetadata(typeof(OrientationListBox)));
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }

    public interface IOrientation
    {
        Orientation Orientation { get; set; }
    }
}