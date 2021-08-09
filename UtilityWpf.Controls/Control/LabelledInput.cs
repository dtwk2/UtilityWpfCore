using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ReactiveUI;

namespace UtilityWpf.Controls
{
    public class LabelledInput : ContentControl
    {
        private const double OpacityLabel = 0.5;

        private Label? theLabel;
        private TextBlock? hintBox;

        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(LabelledInput));
        public static readonly DependencyProperty HintProperty = DependencyProperty.Register("Hint", typeof(string), typeof(LabelledInput));
        public static readonly DependencyProperty HintColorProperty = DependencyProperty.Register("HintColor", typeof(Color), typeof(LabelledInput), new PropertyMetadata(Colors.Gray));

        static LabelledInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelledInput), new FrameworkPropertyMetadata(typeof(LabelledInput)));
        }

        public override void OnApplyTemplate()
        {
            theLabel = GetTemplateChild("PART_Label") as Label ?? throw new NullReferenceException("PART_Label object is null");
            hintBox = GetTemplateChild("HintTextBox") as TextBlock ?? throw new NullReferenceException("HintTextBox object is null");

            this.WhenAnyValue(a => a.LabelText)
                .BindTo(theLabel, a => a.Content);

            this.WhenAnyValue(a => a.Hint)
                .BindTo(hintBox, a => a.Text);

            this.WhenAnyValue(a => a.HintColor)
                .Select(a => new SolidColorBrush { Color = a })
                .BindTo(hintBox, a => a.Foreground);

            if (LabelText != null)
                theLabel.Content = LabelText;
            SetupAnimation();
            base.OnApplyTemplate();
        }

        private void SetupAnimation()
        {
            if (theLabel is not null)
            {
                theLabel.Opacity = OpacityLabel;
                GotFocus += LabelledInput_GotFocus;
                LostFocus += LabelledInput_LostFocus;
            }

            void LabelledInput_LostFocus(object sender, RoutedEventArgs e)
            {
                DoubleAnimation animation = new(theLabel!.Opacity, OpacityLabel, new Duration(TimeSpan.FromSeconds(1)));
                theLabel.BeginAnimation(OpacityProperty, animation);
            }

            void LabelledInput_GotFocus(object sender, RoutedEventArgs e)
            {
                DoubleAnimation animation = new(theLabel!.Opacity, 1, new Duration(TimeSpan.FromSeconds(0.1)));
                theLabel.BeginAnimation(OpacityProperty, animation);
            }
        }


        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public Color HintColor
        {
            get => (Color)GetValue(HintColorProperty);
            set => SetValue(HintColorProperty, value);
        }
    }
}
