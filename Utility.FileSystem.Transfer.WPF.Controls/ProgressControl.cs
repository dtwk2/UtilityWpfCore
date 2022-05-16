using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.FileSystem.Transfer.Abstract;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.WPF.Controls.Abstract;
using Utility.WPF.Reactive;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class ProgressControl : Control, IProgressView
    {
        public static readonly RoutedEvent CompleteEvent = EventManager.RegisterRoutedEvent("Complete", RoutingStrategy.Bubble, typeof(MultiProgress.TimeSpanRoutedEventHandler), typeof(ProgressControl));
        public static readonly DependencyProperty ConfigContentProperty = DependencyProperty.Register(nameof(ConfigContent), typeof(object), typeof(ProgressControl), new PropertyMetadata(ConfigContentChanged));
        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register(nameof(Details), typeof(string), typeof(ProgressControl), new PropertyMetadata("Details about task", DetailsChanged));
        public static readonly DependencyProperty IsCompleteProperty = DependencyProperty.Register(nameof(IsComplete), typeof(bool), typeof(ProgressControl), new PropertyMetadata(false));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ProgressControl), new PropertyMetadata(readOnlyChanged));
        public static readonly DependencyProperty RunCommandProperty = DependencyProperty.Register(nameof(RunCommand), typeof(ICommand), typeof(ProgressControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ShowTransferProperty = DependencyProperty.Register(nameof(ShowTransfer), typeof(bool), typeof(ProgressControl), new PropertyMetadata(showTransferChanged));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ProgressControl), new PropertyMetadata("Progressing", TitleChanged));
        public static readonly DependencyProperty TransfererProperty = DependencyProperty.Register(nameof(Transferer), typeof(ITransferer), typeof(ProgressControl), new PropertyMetadata(TransfererChanged));
        protected readonly Subject<object> ConfigContentControlChanges = new Subject<object>();
        protected readonly Subject<string> DetailChanges = new Subject<string>();
        protected readonly Subject<bool> ReadOnlyChanges = new Subject<bool>();
        protected readonly Subject<bool> ShowTransferChanges = new Subject<bool>();
        protected readonly Subject<Unit> TemplateApplied = new Subject<Unit>();
        protected readonly Subject<string> TitleChanges = new Subject<string>();
        protected readonly Subject<Unit> TransferButtonsClicks = new Subject<Unit>();
        protected readonly Subject<ITransferer> TransferrerChanges = new Subject<ITransferer>();
        protected ContentControl ConfigContentControl;
        protected ProgressBar ProgressBar;
        protected TextBlock TitleTextBlock;
        protected Panel TopPanel;
        protected Button TransferButton;
        private readonly ReplaySubject<TimeSpanRoutedEventArgs> replaySubject = new ReplaySubject<TimeSpanRoutedEventArgs>(1);

        static ProgressControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressControl), new FrameworkPropertyMetadata(typeof(ProgressControl)));
        }

        public ProgressControl()
        {
            RunCommand = ReactiveCommand.Create(() => TransferButtonsClicks.OnNext(Unit.Default));
            ConfigContentControlChanges.CombineLatest<object, Unit, object>(TemplateApplied, (a, b) => a).Subscribe<object>(content => ConfigContentControl.Content = content);
            ShowTransferChanges.CombineLatest<bool, Unit, bool>(TemplateApplied, (a, b) => a).SubscribeOnDispatcher<bool>().Subscribe<bool>(a => TransferButton.Visibility = a ? Visibility.Visible : Visibility.Hidden);
            ReadOnlyChanges.CombineLatest<bool, Unit, bool>(TemplateApplied, (a, b) => a).Subscribe<bool>(a =>
            {
                TransferButton.Visibility = a ? Visibility.Collapsed : Visibility.Visible;
                ConfigContentControl.Visibility = a ? Visibility.Collapsed : Visibility.Visible;
            });
            DetailChanges.CombineLatest<string, Unit, string>(TemplateApplied, (a, b) => a).Subscribe<string>(a => TitleTextBlock.ToolTip = a);
            TitleChanges.CombineLatest(this.LoadedChanges()).Subscribe(a => TitleTextBlock.Text = a.First);
            ScheduleProgress();
            this.LoadedChanges()
            .Subscribe(a =>
            {
                ReadOnlyChanges.OnNext(IsReadOnly);
                ShowTransferChanges.OnNext(ShowTransfer);
            });
        }

        public IObservable<TimeSpan> CompleteEvents => replaySubject.AsObservable<TimeSpanRoutedEventArgs>().Select<TimeSpanRoutedEventArgs, TimeSpan>(a => a.TimeSpan);

        public object ConfigContent
        {
            get => GetValue(ProgressControl.ConfigContentProperty);
            set => SetValue(ProgressControl.ConfigContentProperty, value);
        }

        public string Details
        {
            get => (string)GetValue(ProgressControl.DetailsProperty);
            set => SetValue(ProgressControl.DetailsProperty, value);
        }

        public bool IsComplete
        {
            get => (bool)GetValue(ProgressControl.IsCompleteProperty);
            set => SetValue(ProgressControl.IsCompleteProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(ProgressControl.IsReadOnlyProperty);
            set => SetValue(ProgressControl.IsReadOnlyProperty, value);
        }

        public ICommand RunCommand
        {
            get => (ICommand)GetValue(ProgressControl.RunCommandProperty);
            set => SetValue(ProgressControl.RunCommandProperty, value);
        }

        public bool ShowTransfer
        {
            get => (bool)GetValue(ProgressControl.ShowTransferProperty);
            set => SetValue(ProgressControl.ShowTransferProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(ProgressControl.TitleProperty);
            set => SetValue(ProgressControl.TitleProperty, value);
        }

        public ITransferer Transferer
        {
            get => (ITransferer)GetValue(ProgressControl.TransfererProperty);
            set => SetValue(ProgressControl.TransfererProperty, value);
        }

        public event MultiProgress.TimeSpanRoutedEventHandler Complete
        {
            add => AddHandler(ProgressControl.CompleteEvent, value);
            remove => RemoveHandler(ProgressControl.CompleteEvent, value);
        }

        public override void OnApplyTemplate()
        {
            TransferButton = GetTemplateChild("transferButton") as Button;
            TopPanel = GetTemplateChild("TopPanel") as Panel;
            TransferButton = GetTemplateChild("transferButton") as Button;
            ConfigContentControl = GetTemplateChild("ContentControl1") as ContentControl;
            TransferButton.Command = RunCommand;
            TitleTextBlock = GetTemplateChild("TitleTextBlock") as TextBlock;
            TitleTextBlock.Text = Title;
            ProgressBar = GetTemplateChild("progressBar") as ProgressBar;
            base.OnApplyTemplate();
            TemplateApplied.OnNext(Unit.Default);
        }

        protected void RaiseCompleteEvent(TimeSpan timeSpan)
        {
            TimeSpanRoutedEventArgs spanRoutedEventArgs = new TimeSpanRoutedEventArgs(ProgressControl.CompleteEvent, timeSpan);
            RaiseEvent(spanRoutedEventArgs);
            replaySubject.OnNext(spanRoutedEventArgs);
        }

        protected virtual void ScheduleProgress()
        {
            IObservable<ITransferer> source = TransferButtonsClicks.CombineLatest<Unit, Unit, Unit>(TemplateApplied, (a, b) => a).WithLatestFrom<Unit, ITransferer, ITransferer>(TransferrerChanges, (a, b) => b);
            source.Select(transferer => transferer.Transfer().Scan((date: DateTime.Now, new TimeSpan(), new Progress()), (d, t) => (d.date, DateTime.Now - d.date, t))).Switch<(DateTime, TimeSpan, Progress)>().Subscribe<(DateTime, TimeSpan, Progress)>(a =>
              {
                  (DateTime _, TimeSpan timeSpan2, Progress progress2) = a;
                  (this).Dispatcher.Invoke(() =>
                  {
                      IsComplete = false;
                      ProgressBar.Value = progress2.Percentage;
                      ProgressBar.Tag = progress2.Fraction.ToString("00 %");
                      if (progress2.BytesTransferred != progress2.Total)
                      {
                          return;
                      }

                      IsComplete = true;
                      RaiseCompleteEvent(timeSpan2);
                      TitleTextBlock.Opacity = 0.5;
                  });
              });
            source.Subscribe<ITransferer>(a =>
            {
                TitleTextBlock.Visibility = Visibility.Visible;
                TransferButton.Visibility = Visibility.Collapsed;
            });
        }

        private static void ConfigContentChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
            {
                return;
            }

            progressControl.ConfigContentControlChanges.OnNext(e.NewValue);
        }

        private static void DetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
            {
                return;
            }

            progressControl.DetailChanges.OnNext((string)e.NewValue);
        }

        private static void readOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
            {
                return;
            }

            progressControl.ReadOnlyChanges.OnNext((bool)e.NewValue);
        }

        private static void showTransferChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
            {
                return;
            }

            progressControl.ShowTransferChanges.OnNext((bool)e.NewValue);
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
            {
                return;
            }

            progressControl.TitleChanges.OnNext((string)e.NewValue);
        }

        private static void TransfererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
            {
                return;
            }

            progressControl.TransferrerChanges.OnNext(e.NewValue as ITransferer);
        }
    }
}