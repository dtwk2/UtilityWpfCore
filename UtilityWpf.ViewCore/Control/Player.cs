using System;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using UtilityWpf;

namespace UtilityWpf.View
{
    public class Player : ProgressTimeBar
    {
        //private ToggleButtonEx playPauseButton;
        //private Button cancelButton;
        //private Slider SliderControl;
        //public ProgressTimeBar ProgressTimeBar { get; set; }

        //public UtilityEnum.ProcessState Output
        //{
        //    get { return (UtilityEnum.ProcessState)GetValue(OutputProperty); }
        //    set { SetValue(OutputProperty, value); }
        //}

        //public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(UtilityEnum.ProcessState), typeof(Player), new PropertyMetadata(default(UtilityEnum.ProcessState),ProcessStateChanged));

        //public bool IsPlaying
        //{
        //    get { return (bool)GetValue(IsPlayingProperty); }
        //    //set { SetValue(IsPlayingProperty, value); }
        //}

        //public TimeSpan TimeOut
        //{
        //    get { return (TimeSpan)GetValue(TimeOutProperty); }
        //    set { SetValue(TimeOutProperty, value); }
        //}

        //public double Attribute
        //{
        //    get { return (double)GetValue(AttributeProperty); }
        //    set { SetValue(AttributeProperty, value); }
        //}

        public object ProcessState
        {
            get { return (object)GetValue(ProcessStateProperty); }
            set { SetValue(ProcessStateProperty, value); }
        }

        //public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(int), typeof(Player), new PropertyMetadata(0));

        public static readonly DependencyProperty ProcessStateProperty = DependencyProperty.Register("ProcessState", typeof(object), typeof(Player), new PropertyMetadata(UtilityEnum.ProcessState.Ready, ProcessStateChanged));

        //public static readonly DependencyProperty QueuedItemsProperty = DependencyProperty.Register("QueuedItems", typeof(int), typeof(Player), new PropertyMetadata(0));

        //public static readonly DependencyProperty AttributeProperty = DependencyProperty.Register("Attribute", typeof(double), typeof(Player), new FrameworkPropertyMetadata(0.00, new PropertyChangedCallback(AttributeChanged)));

        //public static readonly DependencyProperty TimeOutProperty = DependencyProperty.Register("TimeOut", typeof(TimeSpan), typeof(Player), new PropertyMetadata(default(TimeSpan), TimeOutChanged));

        //public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register("IsPlaying", typeof(bool), typeof(Player), new FrameworkPropertyMetadata(false,OnIsPlayingChanged));

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(Player));

        //private static void OnIsPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Player).IsPlayingSubject.OnNext((bool)e.NewValue);
        //}

        //private static void AttributeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Player).AttributeChanges.OnNext((double)e.NewValue);
        //}

        //private static void TimeOutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Player).TimeOutChanges.OnNext((TimeSpan)e.NewValue);
        //}

        private static void ProcessStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Player).ProcessStates.OnNext((UtilityEnum.ProcessState)e.NewValue);
        }

        //protected ISubject<bool> IsPlayingSubject = new Subject<bool>();
        protected ISubject<UtilityEnum.ProcessState> ProcessStates = new Subject<UtilityEnum.ProcessState>();

        //protected ISubject<TimeSpan> TimeOutChanges = new Subject<TimeSpan>();
        //protected ISubject<double> AttributeChanges = new Subject<double>();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //playPauseButton = GetTemplateChild("PlayPause") as ToggleButtonEx;
            // cancelButton = GetTemplateChild("Cancel") as Button;

            //playPauseButton.Click += PlayPauseButton_Click;
            //cancelButton.Click += CancelButton_Click;

            //SliderControl = GetTemplateChild("AttributeSlider") as Slider;

            //SliderControl.ValueChanged += SliderControl_ValueChanged;
            //ProgressTimeBar = GetTemplateChild("ProgressTimeBar") as ProgressTimeBar;
            //this.ValueChanged += ProgressTimeBar_ValueChanged;
            //chkbx.Checked += Chkbx_Checked;
        }

        static Player()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Player), new FrameworkPropertyMetadata(typeof(Player)));
        }

        public Player()
        {
            Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/Player.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Style = resourceDictionary["PlayerStyle"] as Style;

            this.SetValue(CancelCommandProperty, new RelayCommand(() =>
             {
                 this.Dispatcher.InvokeAsync(() => ProcessState = UtilityEnum.ProcessState.Terminated, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
             }));
        }

        //protected override void TimeFunction()
        //{
        //    var obs = Observable
        //     .FromEventPattern<RoutedPropertyChangedEventHandler<double>, RoutedPropertyChangedEventArgs<double>>(h => this.ValueChanged += h, h => this.ValueChanged -= h);

        //    var x = obs
        //    .Select(_ => new { _.EventArgs, interval = Observable.Interval(TimeSpan.FromMilliseconds(10)) })
        //    .Where(_ => /*_.EventArgs.OldValue == 0 ||*/ _.EventArgs.OldValue >= _.EventArgs.NewValue)
        //    .Select(aa => aa.interval)
        //    .Switch()
        //    .WithLatestFrom(obs, (a, b) => new { a, b })
        //    .TakeWhile(t => t.b.EventArgs.NewValue < 100)
        //    .Select(t => t.a)
        //    .Subscribe(_ =>
        //    {
        //        this.Dispatcher.InvokeAsync(() => MTime = TimeSpan.FromMilliseconds((long)((double)_) * 10), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //    });
        //}

        //private void SliderControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    SetValue(AttributeProperty, e.NewValue);
        //}

        //private void ProgressTimeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (e.NewValue == 100)
        //        CancelButton_Click(null, null);
        //}

        //private void CancelButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Dispatcher.InvokeAsync(() => CancelEnabled = false, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));

        //    //playPauseButton.Click -= PlayPauseButton_Click;
        //    //playPauseButton.IsChecked = false;
        //    //SetValue(ProcessStateProperty, null);
        //    //playPauseButton.Click += PlayPauseButton_Click;

        //    // playPauseButton.IsChecked = true;

        //}

        //private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    CancelEnabled = true;

        //    //ProgressTimeBar.ValueProperty
        //    SetValue(ProcessStateProperty, (sender as ToggleButtonEx).IsChecked == true ? true : false);
        //}
    }
}