using System;
using System.Collections;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Utility.FileSystem.Transfer.WPF.Controls.Abstract;
using Utility.WPF.Helper;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class MultiProgress : Control
    {
        public static readonly DependencyProperty ProgressViewsProperty = DependencyProperty.Register(nameof(ProgressViews), typeof(IEnumerable), typeof(MultiProgress), new PropertyMetadata(null));
        public static readonly DependencyProperty IsCompleteProperty = DependencyProperty.Register(nameof(IsComplete), typeof(bool), typeof(MultiProgress), new PropertyMetadata(false));
        public static readonly RoutedEvent CompleteEvent = EventManager.RegisterRoutedEvent("Complete", RoutingStrategy.Bubble, typeof(MultiProgress.TimeSpanRoutedEventHandler), typeof(MultiProgress));
        private ContentControl contentControl;
        private TextBlock textBlock;

        static MultiProgress()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiProgress), new FrameworkPropertyMetadata(typeof(MultiProgress)));
        }

        public MultiProgress()
        {
            Loaded += new RoutedEventHandler(MultiStageProgressView_Loaded);
        }

        public int CountDown { get; set; } = 3;

        public IEnumerable ProgressViews
        {
            get => (IEnumerable)GetValue(MultiProgress.ProgressViewsProperty);
            set => SetValue(MultiProgress.ProgressViewsProperty, value);
        }

        public bool IsComplete
        {
            get => (bool)GetValue(MultiProgress.IsCompleteProperty);
            set => SetValue(MultiProgress.IsCompleteProperty, value);
        }

        private void MultiStageProgressView_Loaded(object sender, RoutedEventArgs e)
        {
            contentControl = this.ChildOfType<ContentControl>();
            TextBlock textBlock = new TextBlock
            {
                FontWeight = FontWeights.Bold,
                FontSize = 40.0,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            this.textBlock = textBlock;
            contentControl.Content = this.textBlock;
            int countDown = CountDown;
            Observable.Interval(TimeSpan.FromSeconds(1.0)).Scan<long, int>(countDown, (a, _) => a - 1).Take<int>(countDown).ObserveOnDispatcher<int>().Subscribe<int>(new Action<int>(CountDownChange), async () =>
            {
                IProgressView[] arr = ProgressViews.Cast<IProgressView>().ToArray<IProgressView>();
                IProgressView[] progressViewArray = arr;
                for (int index = 0; index < progressViewArray.Length; ++index)
                {
                    IProgressView progressView = progressViewArray[index];
                    TimeSpan timeSpan = await StepChange(progressView);
                    progressView = null;
                }
                progressViewArray = null;
                RaiseCompleteEvent();
                arr = null;
            });
        }

        protected virtual async Task<TimeSpan> StepChange(IProgressView progressView)
        {
            Task task1 = await Dispatcher.InvokeAsync(Callback);
            TimeSpan task = await progressView.CompleteEvents.Take<TimeSpan>(1).ToTask<TimeSpan>();
            return task;

            static DoubleAnimation GetAnimation()
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    To = new double?(0.0),
                    Duration = (Duration)TimeSpan.FromSeconds(2.0),
                    FillBehavior = FillBehavior.Stop
                };
                return doubleAnimation;
            }

            static DoubleAnimation GetAnimation2()
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    To = new double?(1.0),
                    Duration = (Duration)TimeSpan.FromSeconds(2.0),
                    FillBehavior = FillBehavior.Stop
                };
                return doubleAnimation;
            }

            static Task<EventPattern<object>> ToTask(DoubleAnimation animation)
            {
                return Observable.FromEventPattern(a => animation.Completed += a, a => animation.Completed -= a).Take<EventPattern<object>>(1).ToTask<EventPattern<object>>();
            }

            async Task Callback()
            {
                DoubleAnimation animation = GetAnimation();
                Task<EventPattern<object>> task = ToTask(animation);
                contentControl.BeginAnimation(UIElement.OpacityProperty, animation);
                EventPattern<object> eventPattern1 = await task;
                contentControl.Content = progressView;
                DoubleAnimation animation2 = GetAnimation2();
                task = ToTask(animation2);
                contentControl.BeginAnimation(UIElement.OpacityProperty, animation2);
                progressView.RunCommand.Execute(null);
                EventPattern<object> eventPattern2 = await task;
                animation = null;
                task = null;
                animation2 = null;
            }
        }

        protected virtual void CountDownChange(int a)
        {
            textBlock.Text = a.ToString();
        }

        public event RoutedEventHandler Complete
        {
            add => AddHandler(MultiProgress.CompleteEvent, value);
            remove => RemoveHandler(MultiProgress.CompleteEvent, value);
        }

        private void RaiseCompleteEvent()
        {
            RaiseEvent(new RoutedEventArgs(MultiProgress.CompleteEvent));
        }

        public delegate void TimeSpanRoutedEventHandler(object sender, TimeSpanRoutedEventArgs e);
    }
}
