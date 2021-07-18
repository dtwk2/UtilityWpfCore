using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class SwitchClock : Control
    {
        static SwitchClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchClock), new FrameworkPropertyMetadata(typeof(SwitchClock)));
        }

    }
}
