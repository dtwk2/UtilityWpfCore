using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Animation
{
    public sealed class BorderFix : ContentControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="BorderFix"/> class.
        /// </summary>
        static BorderFix()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderFix), new FrameworkPropertyMetadata(typeof(BorderFix)));
        }
    }
}