using System;
using System.Windows;

namespace UtilityWpf.View
{
    // Based on https://stackoverflow.com/questions/2491941/wpf-tristate-image-button/3676177#3676177
    // answered Sep 9 '10 at 11:42 PJUK

    public class ImageButton : System.Windows.Controls.Button
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageButton() : base()
        {
        }

        #region Dependency Properties

        public double ImageSize
        {
            get { return (double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register("ImageSize", typeof(double), typeof(ImageButton),
            new FrameworkPropertyMetadata(30.0, FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        public Uri Image
        {
            get { return (Uri)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(Uri), typeof(ImageButton),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ImageChanged));

        private static void ImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ImageButton imageButton && e.NewValue is Uri uri))
                return;

            if (imageButton.HoverImage == null)
                imageButton.HoverImage = uri;
        }

        public Uri HoverImage
        {
            get { return (Uri)GetValue(HoverImageProperty); }
            set { SetValue(HoverImageProperty, value); }
        }

        public static readonly DependencyProperty HoverImageProperty =
            DependencyProperty.Register("HoverImage", typeof(Uri), typeof(ImageButton),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        public Uri PressedImage
        {
            get { return (Uri)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }

        public static readonly DependencyProperty PressedImageProperty =
            DependencyProperty.Register("PressedImage", typeof(Uri), typeof(ImageButton),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        public Uri DisabledImage
        {
            get { return (Uri)GetValue(DisabledImageProperty); }
            set { SetValue(DisabledImageProperty, value); }
        }

        public static readonly DependencyProperty DisabledImageProperty =
            DependencyProperty.Register("DisabledImage", typeof(Uri), typeof(ImageButton),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        private static void ImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //  Application.GetResourceStream(new Uri("pack://application:,,," + (Uri)e.NewValue));
        }

        #endregion Dependency Properties
    }
}