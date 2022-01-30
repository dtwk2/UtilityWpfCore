//namespace UtilityWpf.Adorners
//{
//    //-----------------------------------------------------------------------
//    // <copyright file="Placeholder.cs" company="Jeow Li Huan">
//    // Copyright (c) Jeow Li Huan. All rights reserved.
//    // </copyright>
//    //-----------------------------------------------------------------------
//    using System;
//    using System.Collections.Generic;
//    using System.Windows;
//    using System.Windows.Controls;
//    using System.Windows.Controls.Primitives;
//    using System.Windows.Documents;
//    using System.Windows.Media;

//    /// <summary>
//    ///   Represents an adorner that adds placeholder text to a
//    ///   <see cref="T:System.Windows.Controls.TextBox"/>,
//    ///   <see cref="T:System.Windows.Controls.RichTextBox"/> or
//    ///   <see cref="T:System.Windows.Controls.PasswordBox"/>.
//    /// </summary>
//    public class HideEx
//    {

//        public static readonly DependencyProperty HideOnFocusProperty = DependencyProperty.RegisterAttached(
//            "HideOnFocus", typeof(bool), typeof(HideEx), new FrameworkPropertyMetadata(true, OnHideOnFocusChanged));

//        /// <summary>
//        ///   <see langword="true"/> when the placeholder text is visible, <see langword="false" /> otherwise.
//        ///   Used to avoid calling <see cref="M:System.Windows.UIElement.InvalidateVisual"/> unnecessarily.
//        /// </summary>
//        private static bool isPlaceholderVisible;

//        //private static double offset;

//        static HideEx()
//        {
//        }

//        public HideEx(PasswordBox adornedElement) : this((Control)adornedElement)
//        {
//            if (!(adornedElement.IsFocused && (bool)adornedElement.GetValue(HideOnFocusProperty)))
//                adornedElement.PasswordChanged += this.AdornedElement_ContentChanged;
//        }

//        public HideEx(TextBoxBase adornedElement) : this((Control)adornedElement)
//        {
//            if (!(adornedElement.IsFocused && (bool)adornedElement.GetValue(HideOnFocusProperty)))
//                adornedElement.TextChanged += this.AdornedElement_ContentChanged;
//        }

//        protected HideEx(Control adornedElement) //: base(adornedElement)
//        {

//            if ((bool)adornedElement.GetValue(HideOnFocusProperty))
//            {
//                adornedElement.GotFocus += this.AdornedElement_GotFocus;
//                adornedElement.LostFocus += this.AdornedElement_LostFocus;
//            }
//        }

//        #region Attached Property Getters and Setters

//        public static bool GetHideOnFocus(Control adornedElement)
//        {
//            if (adornedElement == null)
//                throw new ArgumentNullException("adornedElement");
//            return (bool)adornedElement.GetValue(HideOnFocusProperty);
//        }

//        public static void SetHideOnFocus(Control adornedElement, bool hideOnFocus)
//        {
//            if (adornedElement == null)
//                throw new ArgumentNullException("adornedElement");
//            adornedElement.SetValue(HideOnFocusProperty, hideOnFocus);
//        }

//        #endregion Attached Property Getters and Setters

//        /// <summary>
//        ///   Draws the content of a <see cref="T:System.Windows.Media.DrawingContext"/> object during the render pass of a <see cref="T:Huan.Windows.Documents.Placeholder"/> element.
//        /// </summary>
//        /// <param name="drawingContext">
//        ///   The <see cref="T:System.Windows.Media.DrawingContext" /> object to draw. This context is provided to the layout system.
//        /// </param>
//        //protected override void OnRender(DrawingContext drawingContext)
//        //{
//        //    Control adornedElement = this.AdornedElement as Control;
//        //    string placeholderText;
//        //    bool hideOnFocus = (bool)adornedElement.GetValue(HideOnFocusProperty);
//        //    if (adornedElement == null ||
//        //        (adornedElement.IsFocused && hideOnFocus) ||
//        //        !this.IsElementEmpty() ||
//        //        string.IsNullOrEmpty(placeholderText = (string)adornedElement.GetValue(HeaderProperty)))
//        //        this.isPlaceholderVisible = false;
//        //    else
//        //    {
//        //        this.isPlaceholderVisible = true;
//        //        Size size = adornedElement.RenderSize;
//        //        double maxHeight = size.Height - adornedElement.BorderThickness.Top - adornedElement.BorderThickness.Bottom - adornedElement.Padding.Top - adornedElement.Padding.Bottom;
//        //        double maxWidth = size.Width - adornedElement.BorderThickness.Left - adornedElement.BorderThickness.Right - adornedElement.Padding.Left - adornedElement.Padding.Right - 4.0;
//        //        if (maxHeight <= 0 || maxWidth <= 0)
//        //            return;
//        //        TextAlignment computedTextAlignment = this.ComputedTextAlignment();
//        //        // Foreground brush does not need to be dynamic. OnRender called when SystemColors changes.
//        //        Brush foreground = SystemColors.GrayTextBrush.Clone();
//        //        foreground.Opacity = adornedElement.Foreground.Opacity;
//        //        Typeface typeface = new Typeface(adornedElement.FontFamily, FontStyles.Oblique, adornedElement.FontWeight, adornedElement.FontStretch);
//        //        FormattedText formattedText = new FormattedText(
//        //            placeholderText,
//        //            CultureInfo.CurrentCulture,
//        //            adornedElement.FlowDirection,
//        //            typeface,
//        //            adornedElement.FontSize,
//        //            foreground)
//        //        {
//        //            TextAlignment = computedTextAlignment,
//        //            MaxTextHeight = maxHeight,
//        //            MaxTextWidth = maxWidth
//        //        };
//        //        double left;
//        //        double top = 0.0;
//        //        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
//        //            left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
//        //        else
//        //            left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;
//        //        switch (adornedElement.VerticalContentAlignment)
//        //        {
//        //            case VerticalAlignment.Top:
//        //            case VerticalAlignment.Stretch:
//        //                top = adornedElement.BorderThickness.Top + adornedElement.Padding.Top;
//        //                break;

//        //            case VerticalAlignment.Bottom:
//        //                top = size.Height - adornedElement.BorderThickness.Bottom - adornedElement.Padding.Bottom - formattedText.Height;
//        //                break;

//        //            case VerticalAlignment.Center:
//        //                top = (size.Height + adornedElement.BorderThickness.Top - adornedElement.BorderThickness.Bottom + adornedElement.Padding.Top - adornedElement.Padding.Bottom - formattedText.Height) / 2.0;
//        //                break;
//        //        }
//        //        if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
//        //        {
//        //            // Somehow everything got drawn reflected. Add a transform to correct.
//        //            drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
//        //            drawingContext.DrawText(formattedText, new Point(left, top + (addOffset ? -formattedText.Height + 2 : 0)));
//        //            drawingContext.Pop();
//        //        }
//        //        else
//        //            drawingContext.DrawText(formattedText, new Point(left, top + (addOffset ? -formattedText.Height - 2 : 0)));
//        //    }
//        //}

//        /// <summary>
//        ///   Adds a <see cref="T:Huan.Windows.Documents.Placeholder"/> to the adorner layer.
//        /// </summary>
//        /// <param name="adornedElement">
//        ///   The adorned element.
//        /// </param>
//        private static void AddAdorner(Control adornedElement)
//        {
//            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
//            if (adornerLayer == null)
//                return;
//            Adorner[] adorners = adornerLayer.GetAdorners(adornedElement);
//            if (adorners != null)
//                foreach (Adorner adorner in adorners)
//                    if (adorner is Placeholder)
//                    {
//                        adorner.InvalidateVisual();
//                        return;
//                    }
//            adornerLayer.Add(adornedElement switch
//            {
//                TextBox => new HideEx(adornedElement),
//                RichTextBox => new HideEx(adornedElement),
//                PasswordBox => new HideEx(adornedElement),
//                _ => throw new NotImplementedException(),
//            });

//            // TextBox is hidden in template. Search for it.
//            //TextBox? templateTextBox = Find<TextBox>(adornedElement);
//            //if (templateTextBox != null)
//            //    SetHeader(templateTextBox, GetHeader(adornedElement));
//        }

//        private static void AdornedElement_Loaded(object sender, RoutedEventArgs e)
//        {
//            Control adornedElement = (Control)sender;
//            //adornedElement.Loaded -= AdornedElement_Loaded;
//            if (string.IsNullOrEmpty(Placeholder.GetHeader(adornedElement)) == false)
//                AddAdorner(adornedElement);
//            else
//            {
//            }
//        }

//        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
//        {
//            if (((string)e.OldValue).Equals((string)e.NewValue, StringComparison.Ordinal))
//                return;
//            if (sender is not Control adornedElement)
//                return;
//            if (adornedElement.IsLoaded)
//                AddAdorner(adornedElement);
//            else
//                adornedElement.Loaded += AdornedElement_Loaded;
//        }

//        private static void OnHideOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
//        {
//            bool hideOnFocus = (bool)e.NewValue;
//            if ((bool)e.OldValue == hideOnFocus)
//                return;
//            var adorner = (sender as Adorner);
//            Control? adornedElement = adorner.AdornedElement as Control;
//            if (adornedElement == null)
//                throw new Exception("bbbrfrr");
//            if (!(adornedElement is TextBox || adornedElement is PasswordBox || adornedElement is RichTextBox))
//            {
//                if (!adornedElement.IsLoaded)
//                    adornedElement.Loaded += ChangeHideOnFocus;
//                else
//                {
//                    TextBox? templateTextBox = Find<TextBox>(adornedElement);
//                    if (templateTextBox != null)
//                        HideEx.SetHideOnFocus(templateTextBox, (bool)e.NewValue);
//                }
//                return;
//            }
//            HideEx? placeholder = null;
//            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
//            if (adornerLayer == null)
//                return;
//            Adorner[] adorners = adornerLayer.GetAdorners(adornedElement);
//            if (adorners != null)
//                foreach (Adorner adorner in adorners)
//                {
//                    placeholder = adorner as Placeholder;
//                    if (placeholder != null)
//                        break;
//                }
//            if (placeholder == null)
//                return;
//            if (adornedElement.IsLoaded)
//            {
//                if (hideOnFocus)
//                {
//                    adornedElement.GotFocus += placeholder.AdornedElement_GotFocus;
//                    adornedElement.LostFocus += placeholder.AdornedElement_LostFocus;
//                    if (adornedElement.IsFocused && placeholder.isPlaceholderVisible)
//                        adorner.InvalidateVisual();
//                }
//                else
//                {
//                    adornedElement.GotFocus -= placeholder.AdornedElement_GotFocus;
//                    adornedElement.LostFocus -= placeholder.AdornedElement_LostFocus;
//                    placeholder.AdornedElement_LostFocus(adornedElement, new RoutedEventArgs(UIElement.LostFocusEvent, placeholder));
//                }
//            }
//            else
//                adornedElement.Loaded += AdornedElement_Loaded;
//        }

//        private static void ChangeHideOnFocus(object sender, RoutedEventArgs e)
//        {
//            if (sender is Control adornedElement && Find<TextBox>(adornedElement) is { } templateTextBox)
//                HideEx.SetHideOnFocus(templateTextBox, HideEx.GetHideOnFocus(adornedElement));
//        }

//        #region Event Handlers

//        private void AdornedElement_GotFocus(object sender, RoutedEventArgs e)
//        {
//            if (AdornedElement is TextBoxBase textBoxBase)
//                textBoxBase.TextChanged -= this.AdornedElement_ContentChanged;
//            else if (AdornedElement is PasswordBox passwordBox)
//                passwordBox.PasswordChanged -= this.AdornedElement_ContentChanged;

//            if (this.isPlaceholderVisible)
//                this.InvalidateVisual();
//        }

//        private void AdornedElement_LostFocus(object sender, RoutedEventArgs e)
//        {
//            if (AdornedElement is TextBoxBase textBoxBase)
//            {
//                textBoxBase.TextChanged += this.AdornedElement_ContentChanged;
//            }
//            else if (AdornedElement is PasswordBox passwordBox)
//            {
//                passwordBox.PasswordChanged += this.AdornedElement_ContentChanged;
//            }
//            if (!this.isPlaceholderVisible && this.IsElementEmpty())
//            {
//                InvalidateArrange();
//            }
//        }

//        private void AdornedElement_ContentChanged(object sender, RoutedEventArgs e)
//        {
//            if (this.isPlaceholderVisible ^ this.IsElementEmpty())
//                this.InvalidateVisual();
//        }

//        #endregion Event Handlers

//        /// <returns>
//        ///   <see langword="true" /> if the content of adorned element is empty; <see langword="false" /> otherwise.
//        /// </returns>
//        private bool IsElementEmpty()
//        {
//            UIElement adornedElement = AdornedElement;

//            if (adornedElement is TextBox textBox)
//                return string.IsNullOrEmpty(textBox.Text);
//            if (adornedElement is PasswordBox passwordBox)
//                return string.IsNullOrEmpty(passwordBox.Password);
//            if (adornedElement is RichTextBox richTextBox)
//            {
//                BlockCollection blocks = richTextBox.Document.Blocks;
//                if (blocks.Count == 0)
//                    return true;
//                if (blocks.Count == 1)
//                {
//                    Paragraph paragraph = blocks.FirstBlock as Paragraph;
//                    if (paragraph == null)
//                        return false;
//                    if (paragraph.Inlines.Count == 0)
//                        return true;
//                    if (paragraph.Inlines.Count == 1)
//                    {
//                        Run run = paragraph.Inlines.FirstInline as Run;
//                        return run != null && string.IsNullOrEmpty(run.Text);
//                    }
//                }
//                return false;
//            }
//            return false;
//        }

//        /// <returns>
//        ///   the computed text alignment of the adorned element.
//        /// </returns>
//        private TextAlignment ComputedTextAlignment()
//        {
//            if (AdornedElement is TextBox textBox)
//            {
//                if (DependencyPropertyHelper.GetValueSource(textBox, TextBox.HorizontalContentAlignmentProperty)
//                    .BaseValueSource != BaseValueSource.Local ||
//                    DependencyPropertyHelper.GetValueSource(textBox, TextBox.TextAlignmentProperty)
//                    .BaseValueSource == BaseValueSource.Local)
//                    // TextAlignment dominates
//                    return textBox.TextAlignment;
//            }
//            else if (AdornedElement is RichTextBox richTextBox)
//            {
//                BlockCollection blocks = richTextBox.Document.Blocks;
//                TextAlignment textAlignment = richTextBox.Document.TextAlignment;
//                if (blocks.Count == 0)
//                    return textAlignment;
//                if (blocks.Count == 1)
//                {
//                    if (blocks.FirstBlock is not Paragraph paragraph)
//                        return textAlignment;
//                    return paragraph.TextAlignment;
//                }
//                return textAlignment;
//            }
//            return (AdornedElement as Control).HorizontalContentAlignment switch
//            {
//                HorizontalAlignment.Left => TextAlignment.Left,
//                HorizontalAlignment.Right => TextAlignment.Right,
//                HorizontalAlignment.Center => TextAlignment.Center,
//                HorizontalAlignment.Stretch => TextAlignment.Justify,
//                _ => TextAlignment.Left,
//            };
//        }

//        /// <summary>
//        ///   Finds a <see cref="T:System.Windows.Controls.Control"/> in the visual tree of the <param name="adornedElement"> using a breadth first search.
//        /// </summary>
//        /// <returns>
//        ///     The <see cref="T:System.Windows.Controls.Control"/> if one is found, <see langword="null"/> if none exists.
//        /// </returns>
//        private static T? Find<T>(Control adornedElement) where T : Control
//        {
//            return Queue(new(new[] { adornedElement }));

//            static T? Queue(Queue<DependencyObject> queue)
//            {
//                while (queue.Count > 0)
//                {
//                    switch (queue.Dequeue())
//                    {
//                        case T templateTextBox:
//                            return templateTextBox;

//                        case { } element:
//                            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); ++i)
//                                queue.Enqueue(VisualTreeHelper.GetChild(element, i));
//                            break;
//                    }
//                }
//                return null;
//            }
//        }
//    }
//}