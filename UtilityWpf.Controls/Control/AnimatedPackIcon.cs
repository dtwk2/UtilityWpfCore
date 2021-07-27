using MaterialDesignThemes.Wpf;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace UtilityWpf.Controls
{
    public class AnimatedPackIcon : PackIcon
    {
        private PackIconKind newElement;
        bool flag = false;
        private Storyboard storyboardOld = new();
        private readonly Storyboard storyboardNew = new();
        ReplaySubject<PackIconKind> completions = new();
        private static PackIconKind kind;


        public static readonly DependencyProperty AnimationTimeProperty =
    DependencyProperty.Register("AnimationTime", typeof(int), typeof(AnimatedPackIcon), new PropertyMetadata(3000));

        static AnimatedPackIcon()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedPackIcon), new FrameworkPropertyMetadata(typeof(AnimatedPackIcon)));
            KindProperty.OverrideMetadata(typeof(AnimatedPackIcon),
      new FrameworkPropertyMetadata(PackIconKind.None,
          FrameworkPropertyMetadataOptions.None,
          null, /* property changed callback */
          Coerce, /* coerce value callback */
          false /* is animation prohibited */
          ));
        }


        public int AnimationTime
        {
            get { return (int)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }


        private static object Coerce(DependencyObject d, object baseValue)
        {
            kind = (PackIconKind)baseValue;

            if (d is AnimatedPackIcon { flag: false } icon)
            {
                icon.StartStories(PackIconKind.None, kind);
                return PackIconKind.None;
            }


            return kind;
        }


        public override void OnApplyTemplate()
        {
            storyboardOld.SelectCompletions().Select(a => newElement).Subscribe(completions.OnNext);
            completions.Subscribe(a =>
            {
                if (flag == true)
                {
                    Opacity = 0;
                    Kind = a;
                    StartStory(this, storyboardNew, true, (int)(AnimationTime / 2d));
                }
                flag = false;

            }, e => flag = false);


            base.OnApplyTemplate();
        }



        public void StartStories(PackIconKind oldElement, PackIconKind newElement)
        {
            if (flag)
                return;

            this.newElement = newElement;

            flag = true;
            if (oldElement.Equals(default) == false)
            {
                Kind = oldElement;
                StartStory(this, storyboardOld, false, (int)(AnimationTime / 2d));
            }
            else
            {
                completions.OnNext(newElement);
            }
        }

        public static void StartStory(UIElement element, Storyboard storyboard, bool addRemove = true, int animationTime = 500)
        {
            if (element == null)
                return;

            storyboard.Children.Clear();
            TimeSpan duration = TimeSpan.FromMilliseconds(animationTime);
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            { From = addRemove ? 0 : 1, To = addRemove ? 1 : 0, Duration = new Duration(duration), EasingFunction = new CubicEase { } };
            Storyboard.SetTarget(fadeOutAnimation, element);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity", addRemove ? 0 : 1));
            storyboard.Children.Add(fadeOutAnimation);

            storyboard.Begin();
        }
    }

}