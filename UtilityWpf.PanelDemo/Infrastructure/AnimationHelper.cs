﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UtilityWpf.PanelDemo
{
    /// <example>
    /// 
    /// </summary>
    public class AnimationHelper
    {
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
