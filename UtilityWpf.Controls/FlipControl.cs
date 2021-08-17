using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace UtilityWpf.Controls
{
    /// <summary>
    /// The flip control from Elysium
    /// </summary>
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_PreviousContent", Type = typeof(ContentControl))]
    public class FlipControl : Selector
    {
        #region Dependency Properties

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration",
            typeof(TimeSpan),
            typeof(FlipControl),
            new PropertyMetadata(TimeSpan.FromSeconds(1)));

        public static readonly DependencyProperty IntegerProperty = DependencyProperty.Register(
            "Integer",
            typeof(int?),
            typeof(FlipControl),
            new PropertyMetadata(null, OnIntegerChanged));

        private static readonly DependencyPropertyKey IsFlippingPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsFlipping",
            typeof(bool),
            typeof(FlipControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsFlippingProperty = IsFlippingPropertyKey.DependencyProperty;

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            "Offset",
            typeof(double),
            typeof(FlipControl),
            new PropertyMetadata(10d));

        public static readonly DependencyProperty PreviousSelectedIndexProperty = DependencyProperty.Register(
            "PreviousSelectedIndex",
            typeof(int),
            typeof(FlipControl),
            new PropertyMetadata(0));

        public static readonly DependencyProperty PreviousSelectedItemProperty = DependencyProperty.Register(
            "PreviousSelectedItem",
            typeof(object),
            typeof(FlipControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedIndexProxyProperty = DependencyProperty.Register(
            "SelectedIndexProxy",
            typeof(int),
            typeof(FlipControl),
            new PropertyMetadata(0, OnSelectedIndexProxyChanged));

        #endregion

        #region Fields

        private ContentControl content;
        private NotifyCollectionChangedEventHandler eventHandler;
        private DateTime lastIntegerChanged;
        private ContentControl previousContent;
        private int oldCount;
        private Storyboard storyboard;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises static members of the <see cref="FlipControl"/> class.
        /// </summary>
        static FlipControl()
        {
            ItemsSourceProperty.OverrideMetadata(
                typeof(FlipControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemsSourceChanged)));
            SelectedIndexProperty.OverrideMetadata(
                typeof(FlipControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedIndexChanged)));
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FlipControl"/> class.
        /// </summary>
        public FlipControl()
        {
            Loaded += OnLoaded;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the integer.
        /// </summary>
        public int? Integer
        {
            get { return (int?)GetValue(IntegerProperty); }
            set { SetValue(IntegerProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether is flipping.
        /// </summary>
        public bool IsFlipping
        {
            get { return (bool)GetValue(IsFlippingProperty); }
            private set { SetValue(IsFlippingPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the previous selected index.
        /// </summary>
        public int PreviousSelectedIndex
        {
            get { return (int)GetValue(PreviousSelectedIndexProperty); }
            set { SetValue(PreviousSelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the previous selected item.
        /// </summary>
        public object PreviousSelectedItem
        {
            get { return GetValue(PreviousSelectedItemProperty); }
            set { SetValue(PreviousSelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected index proxy.
        /// </summary>
        public int SelectedIndexProxy
        {
            get { return (int)GetValue(SelectedIndexProxyProperty); }
            set { SetValue(SelectedIndexProxyProperty, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {

            content = (ContentControl)GetTemplateChild("PART_Content");
            previousContent = (ContentControl)GetTemplateChild("PART_PreviousContent");
            previousContent.Visibility = Visibility.Collapsed;
            base.OnApplyTemplate();

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// The on flipping completed.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        protected virtual void OnFlippingCompleted(object sender, EventArgs e)
        {
            try
            {
                ClockGroup clockGroup = (ClockGroup)sender;
                Storyboard storyboard = (Storyboard)clockGroup.Timeline;

                IsFlipping = false;

                if (storyboard != null)
                {
                    storyboard.Stop();
                    storyboard.Remove();

                    if (ItemsSource != null)
                    {
                        int count = ItemsSource.Cast<object>().Count();

                        if (SelectedIndexProxy == count - 1)
                        {
                            SelectedIndex = SelectedIndexProxy;
                        }
                        else
                        {
                            SelectedIndexProxy = count - 1;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The on selection changed.
        /// </summary>
        /// <param name="e"> The e. </param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            try
            {
                PreviousSelectedItem = e.RemovedItems.Cast<object>().FirstOrDefault();

                var items = ItemsSource.Cast<object>().ToList();
                PreviousSelectedIndex = items.IndexOf(PreviousSelectedItem);

                if (PreviousSelectedItem != null && previousContent != null)
                {
                    previousContent.Visibility = Visibility.Visible;
                }

                base.OnSelectionChanged(e);

                int count = ItemsSource.Cast<object>().Count();
                int selectedIndex = items.IndexOf(SelectedItem);

                if (content != null && previousContent != null && count > 0)
                {
                    TimeSpan flipDuration = new TimeSpan(Duration.Ticks / count);

                    Storyboard storyboard = null;

                    if (selectedIndex <= PreviousSelectedIndex)
                    {
                        storyboard = GetStoryboard(content, previousContent, flipDuration, -Offset);
                    }
                    else
                    {
                        storyboard = GetStoryboard(content, previousContent, flipDuration, Offset);
                    }

                    storyboard.Begin();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The on loaded.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            Animate();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The on integer changed.
        /// </summary>
        /// <param name="dependencyObject"> The dependency object. </param>
        /// <param name="e"> The e. </param>
        private static void OnIntegerChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            FlipControl flipControl = (FlipControl)dependencyObject;

            if (flipControl.Integer.HasValue)
            {
                int? oldInteger = (int?)e.OldValue;
                int newInteger = flipControl.Integer.Value;

                flipControl.AnimateInteger(oldInteger.HasValue ? oldInteger.Value : 0, newInteger);
            }
        }

        /// <summary>
        /// The on items source changed.
        /// </summary>
        /// <param name="dependencyObject"> The dependency object. </param>
        /// <param name="e"> The e. </param>
        private static void OnItemsSourceChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            FlipControl flipControl = (FlipControl)dependencyObject;

            if (flipControl.eventHandler == null)
            {
                flipControl.eventHandler =
                    (sender, e2) =>
                    {
                        flipControl.Animate();
                    };
            }

            INotifyCollectionChanged oldCollection = e.OldValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= flipControl.eventHandler;
            }

            INotifyCollectionChanged newCollection = e.NewValue as INotifyCollectionChanged;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += flipControl.eventHandler;
            }

            flipControl.Animate();
        }

        /// <summary>
        /// The on selected index changed.
        /// </summary>
        /// <param name="dependencyObject"> The dependency object. </param>
        /// <param name="e"> The Event Arguments. </param>
        private static void OnSelectedIndexChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            FlipControl flipControl = (FlipControl)dependencyObject;

            try
            {
                flipControl.SelectedIndexProxy = flipControl.SelectedIndex;
            }
            catch
            {
            }
        }

        /// <summary>
        /// The on selected index proxy changed.
        /// </summary>
        /// <param name="dependencyObject"> The dependency object. </param>
        /// <param name="e"> The Event Arguments. </param>
        private static void OnSelectedIndexProxyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            FlipControl flipControl = (FlipControl)dependencyObject;

            try
            {
                flipControl.SelectedIndex = flipControl.SelectedIndexProxy;
            }
            catch
            {
            }
        }

        /// <summary>
        /// The get storyboard.
        /// </summary>
        /// <param name="target1"> The target 1. </param>
        /// <param name="target2"> The target 2. </param>
        /// <param name="duration"> The duration. </param>
        /// <param name="offset"> The offset. </param>
        /// <returns> The <see cref="Storyboard"/>. </returns>
        private static Storyboard GetStoryboard(
            DependencyObject target1,
            DependencyObject target2,
            TimeSpan duration,
            double offset)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation doubleAnimation1 = new DoubleAnimation()
            {
                Duration = duration,
                From = 0,
                To = 1
            };
            Storyboard.SetTarget(doubleAnimation1, target1);
            Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath("(UIElement.Opacity)"));
            storyboard.Children.Add(doubleAnimation1);

            DoubleAnimation doubleAnimation2 = new DoubleAnimation()
            {
                Duration = duration,
                From = 1,
                To = 0
            };
            Storyboard.SetTarget(doubleAnimation2, target2);
            Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(UIElement.Opacity)"));
            storyboard.Children.Add(doubleAnimation2);

            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames1 = new DoubleAnimationUsingKeyFrames();
            doubleAnimationUsingKeyFrames1.KeyFrames.Add(
                new SplineDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                    Value = offset
                });
            doubleAnimationUsingKeyFrames1.KeyFrames.Add(
                new SplineDoubleKeyFrame()
                {
                    KeySpline = new KeySpline(0.5, 0, 1, 0.75),
                    KeyTime = KeyTime.FromTimeSpan(duration),
                    Value = 0
                });
            Storyboard.SetTarget(doubleAnimationUsingKeyFrames1, target1);
            Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(doubleAnimationUsingKeyFrames1);

            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames();
            doubleAnimationUsingKeyFrames2.KeyFrames.Add(
                new SplineDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                    Value = 0
                });
            doubleAnimationUsingKeyFrames2.KeyFrames.Add(
                new SplineDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(duration),
                    Value = -offset
                });
            Storyboard.SetTarget(doubleAnimationUsingKeyFrames2, target2);
            Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames2, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(doubleAnimationUsingKeyFrames2);

            return storyboard;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The animate.
        /// </summary>
        private void Animate()
        {
            try
            {
                if (storyboard != null)
                {
                    storyboard.Stop();
                    storyboard.Remove();
                }

                PreviousSelectedIndex = -1;
                PreviousSelectedItem = null;
                SelectedIndexProxy = 0;

                if (ItemsSource != null && content != null)
                {

                    var count = ItemsSource.Cast<object>().Count();
                    
                    oldCount = ItemsSource.Cast<int>().LastOrDefault();
                    storyboard = new Storyboard();
                    Int32Animation intAnimation = new Int32Animation()
                    {
                        Duration = Duration,
                        FillBehavior = FillBehavior.HoldEnd,
                        From = 0,
                        To = count - 1
                    };
                    Storyboard.SetTarget(intAnimation, this);
                    Storyboard.SetTargetProperty(intAnimation, new PropertyPath("SelectedIndexProxy"));
                    storyboard.Children.Add(intAnimation);

                    IsFlipping = true;
                    storyboard.Completed += OnFlippingCompleted;
                    storyboard.Begin();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The animate integer.
        /// </summary>
        /// <param name="oldInteger"> The old integer. </param>
        /// <param name="newInteger"> The new integer. </param>
        private void AnimateInteger(int oldInteger, int newInteger)
        {
            try
            {
                if (newInteger >= 0 && oldInteger >= 0)
                {
                    if (newInteger > oldInteger)
                    {
                        if (DateTime.Now - lastIntegerChanged < TimeSpan.FromMilliseconds(500))
                        {
                            ItemsSource = Enumerable.Range(0, newInteger + 1);
                        }
                        else
                        {
                            ItemsSource = Enumerable.Range(oldInteger, newInteger - oldInteger + 1);
                        }
                    }
                    else
                    {
                        if (DateTime.Now - lastIntegerChanged < TimeSpan.FromMilliseconds(500))
                        {
                            ItemsSource = Enumerable.Range(newInteger, oldInteger + 1).Reverse();
                        }
                        else
                        {
                            ItemsSource = Enumerable.Range(newInteger, oldInteger - newInteger + 1).Reverse();
                        }
                    }
                }

                lastIntegerChanged = DateTime.Now;
            }
            catch
            {
            }
        }

        #endregion
    }
}
