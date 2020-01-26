using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class SwitchView : Control
    {

        static SwitchView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchView), new FrameworkPropertyMetadata(typeof(SwitchView)));
        }
    }
}
