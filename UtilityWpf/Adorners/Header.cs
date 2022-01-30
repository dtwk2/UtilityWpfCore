namespace UtilityWpf.Adorners
{
    //-----------------------------------------------------------------------
    // <copyright file="Placeholder.cs" company="Jeow Li Huan">
    // Copyright (c) Jeow Li Huan. All rights reserved.
    // </copyright>
    //-----------------------------------------------------------------------
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    ///   Represents an adorner that adds placeholder text to a
    ///   <see cref="T:System.Windows.Controls.TextBox"/>,
    ///   <see cref="T:System.Windows.Controls.RichTextBox"/> or
    ///   <see cref="T:System.Windows.Controls.PasswordBox"/>.
    /// </summary>
    public class Header : Adorner
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(Header), new FrameworkPropertyMetadata(string.Empty, OnHeaderChanged));

        /// <summary>
        ///   <see langword="true"/> when the placeholder text is visible, <see langword="false" /> otherwise.
        ///   Used to avoid calling <see cref="M:System.Windows.UIElement.InvalidateVisual"/> unnecessarily.
        /// </summary>
        private bool isPlaceholderVisible;

        static Header()
        {
            //IsHitTestVisibleProperty.OverrideMetadata(typeof(Header), new FrameworkPropertyMetadata(false));
            //ClipToBoundsProperty.OverrideMetadata(typeof(Header), new FrameworkPropertyMetadata(true));
        }

        protected Header(FrameworkElement adornedElement) : base(adornedElement)
        {
        }

        #region Attached Property Getters and Setters

        public static string GetText(FrameworkElement adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            return (string)adornedElement.GetValue(TextProperty);
        }

        public static void SetText(FrameworkElement adornedElement, string placeholderText)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            //offset = -adornedElement.Height;
            adornedElement.SetValue(TextProperty, placeholderText);
        }

        #endregion Attached Property Getters and Setters

        /// <summary>
        ///   Draws the content of a <see cref="T:System.Windows.Media.DrawingContext"/> object during the render pass of a <see cref="T:Huan.Windows.Documents.Placeholder"/> element.
        /// </summary>
        /// <param name="drawingContext">
        ///   The <see cref="T:System.Windows.Media.DrawingContext" /> object to draw. This context is provided to the layout system.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            string placeholderText;
            //bool hideOnFocus = (bool)adornedElement.GetValue(HideOnFocusProperty);
            if (string.IsNullOrEmpty(placeholderText = (string)this.AdornedElement.GetValue(TextProperty)))
            {
                this.isPlaceholderVisible = false;
                return;
            }

            if (AdornedElement is Control adornedElement)
            {
                this.isPlaceholderVisible = true;
                Size size = adornedElement.RenderSize;
                double maxHeight = size.Height - adornedElement.BorderThickness.Top - adornedElement.BorderThickness.Bottom - adornedElement.Padding.Top - adornedElement.Padding.Bottom;
                double maxWidth = size.Width - adornedElement.BorderThickness.Left - adornedElement.BorderThickness.Right - adornedElement.Padding.Left - adornedElement.Padding.Right - 4.0;
                if (maxHeight <= 0 || maxWidth <= 0)
                    return;
                TextAlignment computedTextAlignment = this.ComputedTextAlignment();
                // Foreground brush does not need to be dynamic. OnRender called when SystemColors changes.
                Brush foreground = SystemColors.GrayTextBrush.Clone();
                foreground.Opacity = adornedElement.Foreground.Opacity;
                Typeface typeface = new Typeface(adornedElement.FontFamily, FontStyles.Normal, adornedElement.FontWeight, adornedElement.FontStretch);
                FormattedText formattedText = new FormattedText(
                    placeholderText,
                    CultureInfo.CurrentCulture,
                    adornedElement.FlowDirection,
                    typeface,
                    adornedElement.FontSize,
                    foreground)
                {
                    TextAlignment = computedTextAlignment,
                    MaxTextHeight = maxHeight,
                    MaxTextWidth = maxWidth
                };
                double left, top = 0.0;
                if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                    left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
                else
                    left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;

                if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
                {
                    // Somehow everything got drawn reflected. Add a transform to correct.
                    drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
                    drawingContext.DrawText(formattedText, new Point(left, top + -formattedText.Height - 2));
                    drawingContext.Pop();
                }
                else
                    drawingContext.DrawText(formattedText, new Point(left, top + -formattedText.Height - 2));
            }
            else if (AdornedElement is FrameworkElement)
            {
                this.isPlaceholderVisible = true;
                Size size = AdornedElement.RenderSize;
                double maxHeight = size.Height;
                double maxWidth = size.Width - 4.0;
                if (maxHeight <= 0 || maxWidth <= 0)
                    return;
                TextAlignment computedTextAlignment = this.ComputedTextAlignment();
                // Foreground brush does not need to be dynamic. OnRender called when SystemColors changes.
                Brush foreground = SystemColors.GrayTextBrush.Clone();

                Typeface typeface = new Typeface(new FontFamily(), FontStyles.Normal, new FontWeight(), new FontStretch());
                FormattedText formattedText = new FormattedText(
                    placeholderText,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    12,
                    foreground)
                {
                    TextAlignment = computedTextAlignment,
                    MaxTextHeight = maxHeight,
                    MaxTextWidth = maxWidth
                };
                double left, top = 0.0;
                left = 2.0;
                drawingContext.DrawText(formattedText, new Point(left, top + -formattedText.Height - 2));
            }
            /*   (adornedElement.IsFocused&& hideOnFocus) ||*/
            //!this.IsElementEmpty() ||
        }

        /// <summary>
        ///   Adds a <see cref="T:Huan.Windows.Documents.Placeholder"/> to the adorner layer.
        /// </summary>
        /// <param name="adornedElement">
        ///   The adorned element.
        /// </param>
        private static void AddAdorner(FrameworkElement adornedElement)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            if (adornerLayer == null)
                return;
            Adorner[] adorners = adornerLayer.GetAdorners(adornedElement);
            if (adorners != null)
                foreach (Adorner adorner in adorners)
                    if (adorner is Header)
                    {
                        adorner.InvalidateVisual();
                        return;
                    }
            adornerLayer.Add(new Header(adornedElement));
        }

        private static void AdornedElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement adornedElement = (FrameworkElement)sender;
            //adornedElement.Loaded -= AdornedElement_Loaded;
            if (string.IsNullOrEmpty(Header.GetText(adornedElement)) == false)
                AddAdorner(adornedElement);
            else
            {
            }
        }

        private static void OnHeaderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (((string)e.OldValue).Equals((string)e.NewValue, StringComparison.Ordinal))
                return;
            if (sender is not FrameworkElement adornedElement)
                return;
            if (adornedElement.IsLoaded)
                AddAdorner(adornedElement);
            else
                adornedElement.Loaded += AdornedElement_Loaded;
        }

        #region Event Handlers

        private void AdornedElement_ContentChanged(object sender, RoutedEventArgs e)
        {
            if (this.isPlaceholderVisible)
                this.InvalidateVisual();
        }

        #endregion Event Handlers

        /// <returns>
        ///   the computed text alignment of the adorned element.
        /// </returns>
        private TextAlignment ComputedTextAlignment()
        {
            return (AdornedElement as Control)?.HorizontalContentAlignment switch
            {
                HorizontalAlignment.Left => TextAlignment.Left,
                HorizontalAlignment.Right => TextAlignment.Right,
                HorizontalAlignment.Center => TextAlignment.Center,
                HorizontalAlignment.Stretch => TextAlignment.Justify,
                _ => TextAlignment.Left,
            };
        }
    }
}