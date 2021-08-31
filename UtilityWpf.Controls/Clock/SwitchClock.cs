using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    public class SwitchClock : Control
    {
        static SwitchClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchClock), new FrameworkPropertyMetadata(typeof(SwitchClock)));
        }

    }
}
