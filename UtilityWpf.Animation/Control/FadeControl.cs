using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace UtilityWpf.Animation
{
    public class FadeControl : ContentControl
    {
        private bool flag = false;
        private readonly Storyboard storyboardOld = new Storyboard();
        private Storyboard storyboardNew = new Storyboard();
        private ReplaySubject<EventArgs> completions = new ReplaySubject<EventArgs>();
        private UIElement newElement = null;

        public static readonly DependencyProperty FadeCommandProperty = DependencyProperty.Register(
            nameof(FadeCommand), typeof(ICommand), typeof(FadeControl), new PropertyMetadata(default(ICommand)));

        public int AnimationTime
        {
            get { return (int)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }

        public static readonly DependencyProperty AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(int), typeof(FadeControl), new PropertyMetadata(1000));

        static FadeControl()
        {
            FadeControl.ContentProperty.OverrideMetadata(typeof(FadeControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, Changed, null, true
          ));
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FadeControl).StartStories(e.OldValue as UIElement, e.NewValue as UIElement);
        }

        public FadeControl()
        {
            FadeCommand = new FadeCommand(this);

            //this.StartStories(null, this.Content as UIElement);
        }

        public override void OnApplyTemplate()
        {
            storyboardOld.SelectCompletions().Subscribe(completions.OnNext);
            completions.Subscribe(a =>
            {
                if (flag == true)
                {
                    newElement.Opacity = 0;

                    StartStory(newElement, storyboardNew, true, (int)(AnimationTime / 2d));
                }
                flag = false;
            }, e => flag = false);

            base.OnApplyTemplate();
        }

        public ICommand FadeCommand
        {
            get { return (ICommand)GetValue(FadeCommandProperty); }
            set { SetValue(FadeCommandProperty, value); }
        }

        public void StartStories(UIElement oldElement, UIElement newElement)
        {
            this.newElement = newElement ?? oldElement;
            if (flag)
                return;

            flag = true;
            if (oldElement != null)
                this.Content = oldElement;
            if (oldElement != null)
                StartStory(oldElement, storyboardOld, false, (int)(AnimationTime / 2d));
            else
            {
                completions.OnNext(default);
            }
        }

        public static void StartStory(UIElement element, Storyboard storyboard, bool addRemove = true, int animationTime = 500)
        {
            if (element == null)
                return;

            storyboard.Children.Clear();
            TimeSpan duration = TimeSpan.FromMilliseconds(animationTime);
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(fromValue: addRemove ? 0 : 1, toValue: addRemove ? 1 : 0, duration: new Duration(duration))
            { EasingFunction = new SineEase { } };
            Storyboard.SetTarget(fadeOutAnimation, element);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeOutAnimation);

            storyboard.Begin();
        }
    }

    internal class FadeCommand : ICommand
    {
        private readonly FadeControl fadeControl;

        public event EventHandler CanExecuteChanged;

        public FadeCommand(FadeControl fadeControl) => this.fadeControl = fadeControl;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            fadeControl.StartStories(fadeControl.Content as UIElement, parameter as UIElement);
        }
    }
}