﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UtilityWpf.Controls.Panels
{
    /// <example>
    /// 
    /// </summary>
    public class AnimationHelper
    {

        public static void Animate(UIElement parent, UIElement child, Rect rect)
        {
            if (child.RenderTransform is not TranslateTransform translateTransform)
            {
                child.RenderTransform = translateTransform = new TranslateTransform();
            }
            var translationPoint = child.TranslatePoint(new Point(), parent);
            child.RenderTransformOrigin = translationPoint;

            child.Arrange(new Rect(new Point(translationPoint.X, translationPoint.Y), rect.Size));

            Animate(translateTransform, translationPoint, rect.Location);

        }

        public static void Animate(TranslateTransform trans, Point translationPoint, Point combinedPoint)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000))
            {
                IsCumulative = false,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
            };

            animation.From = translationPoint.X;
            animation.To = combinedPoint.X - translationPoint.X;
            trans.BeginAnimation(TranslateTransform.XProperty, animation, HandoffBehavior.SnapshotAndReplace);

            animation.From = translationPoint.Y;
            animation.To = combinedPoint.Y - translationPoint.Y;
            trans.BeginAnimation(TranslateTransform.YProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}
