using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.Common.Behavior
{
    /// <summary>
    /// https://stackoverflow.com/questions/1639505/wpf-scrollviewer-scroll-amount
    /// </summary>

    //    Here's a simple, complete and working WPF ScrollViewer class that has a data-bindable SpeedFactor property for adjusting the mouse wheel sensitivity. Setting SpeedFactor to 1.0 means identical behavior to the WPF ScrollViewer. The default value for the dependency property is 2.5, which allows for very speedy wheel scrolling.

    //Of course, you can also create additional useful features by binding to the SpeedFactor property itself, i.e., to easily allow the user to control the multiplier.

    public class WheelSpeedScrollViewer : ScrollViewer
    {
        public static readonly DependencyProperty SpeedFactorProperty =
            DependencyProperty.Register(nameof(SpeedFactor),
                                        typeof(Double),
                                        typeof(WheelSpeedScrollViewer),
                                        new PropertyMetadata(2.5));

        public Double SpeedFactor
        {
            get { return (Double)GetValue(SpeedFactorProperty); }
            set { SetValue(SpeedFactorProperty, value); }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (!e.Handled &&
                ScrollInfo is ScrollContentPresenter scp &&
                ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                scp.SetVerticalOffset(VerticalOffset - e.Delta * SpeedFactor);
                e.Handled = true;
            }
        }
    };
}
