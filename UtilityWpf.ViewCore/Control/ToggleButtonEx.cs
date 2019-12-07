using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.View
{
    public class ToggleButtonEx : ToggleButton
    {
        public static readonly DependencyProperty FrontImageSourceProperty = DependencyProperty.Register("FrontImageSource", typeof(Uri), typeof(ToggleButtonEx), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFrontImageSourceChanged)));

        public static readonly DependencyProperty BackImageSourceProperty = DependencyProperty.Register("BackImageSource", typeof(Uri), typeof(ToggleButtonEx), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackImageSourceChanged)));

        public System.Windows.Controls.Image MainImage { get; set; }

        public Uri FrontImageSource
        {
            get { return (Uri)GetValue(FrontImageSourceProperty); }
            set { SetValue(FrontImageSourceProperty, value); }
        }

        public Uri BackImageSource
        {
            get { return (Uri)GetValue(BackImageSourceProperty); }
            set { SetValue(BackImageSourceProperty, value); }
        }

        static ToggleButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonEx), new FrameworkPropertyMetadata(typeof(ToggleButtonEx)));
        }

        public ToggleButtonEx() : base()
        {
            Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/ToggleButtonEx.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Style = resourceDictionary["ToggleExStyle"] as Style;
        }

        private static void OnFrontImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as ToggleButtonEx ).
            //if ((d as ToggleButtonEx).MainImage != null)
            //    (d as ToggleButtonEx).MainImage.Source = new System.Windows.Media.Imaging.BitmapImage((Uri)e.NewValue);
        }

        // the inherits property ensure child elements inherit this property FrameworkPropertyMetadataOptions.Inherits,

        private static void OnBackImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if ((d as ToggleButtonEx).MainImage != null)
            //    (d as ToggleButtonEx).MainImage.Source = new System.Windows.Media.Imaging.BitmapImage((Uri)e.NewValue);
        }
    }
}