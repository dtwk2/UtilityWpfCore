using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.View
{
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
    {

        private object content;

        public static readonly DependencyProperty UnCheckedContentProperty = DependencyProperty.Register(
            "UnCheckedContent",
            typeof(object),
            typeof(ToggleButton),
            new FrameworkPropertyMetadata(null));

        static ToggleButton()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(typeof(ToggleButton)));
            //System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty.OverrideMetadata(typeof(UtilityWpf.View.ToggleButton), new PropertyMetadata(true, Changed));
        }

        public ToggleButton()
        {
            this.Unchecked += ToggleButton_;
            this.Checked += ToggleButton_;
        }

        private void ToggleButton_(object sender, RoutedEventArgs e)
        {
            if (!this.IsChecked ?? false)
                this.content = this.Content;
            this.Content = this.IsChecked ?? false ? this.content ?? this.Content: this.UnCheckedContent;

        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ToggleButton tButton && e.NewValue is bool b))
                return;

        }

        public object UnCheckedContent
        {
            get { return (object)GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }



    }
}