using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;

namespace UtilityWpf.Controls.Browser
{
    /// <summary>
    /// Interaction logic for PathBrowser.xaml
    /// </summary>
    public abstract class PathBrowser : Control
    {
        protected readonly ReplaySubject<object> textBoxContentChanges = new(1);
        protected readonly ReplaySubject<TextBox> textBoxSubject = new(1);
        protected readonly ReplaySubject<string> textChanges = new(1);
        protected readonly ReplaySubject<Unit> applyTemplateSubject = new(1);

        protected Button ButtonOne;
        protected ContentControl ContentControlOne;
        protected Label LabelOne;
        protected TextBox TextBoxOne;

        public static readonly DependencyProperty TextBoxContentProperty =
            DependencyProperty.Register("TextBoxContent", typeof(object), typeof(PathBrowser), new PropertyMetadata(null, TextBoxContentChanged));

        public static readonly DependencyProperty SetPathProperty =
            DependencyProperty.Register("SetPath", typeof(ICommand), typeof(PathBrowser), new PropertyMetadata(null));

        public static readonly RoutedEvent TextChangeEvent = EventManager.RegisterRoutedEvent("TextChange", RoutingStrategy.Bubble, typeof(TextChangeRoutedEventHandler), typeof(PathBrowser));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(PathBrowser), new PropertyMetadata(null));

        static PathBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathBrowser), new FrameworkPropertyMetadata(typeof(PathBrowser)));
        }

        public PathBrowser()
        {
            applyTemplateSubject
                .CombineLatest(textBoxContentChanges, (a, b) => b)
                .Subscribe(content =>
                {
                    (ContentControlOne ?? throw new NullReferenceException("ContentControlOne is null")).Content = content;
                    (content as DependencyObject ?? throw new NullReferenceException("content is null"))
                        .FindChildren<TextBox>()
                        .ToObservable()
                        .Merge(Observable.Return(content as TextBox).WhereNotNull())
                        .Subscribe(textBoxSubject.OnNext);
                });

            textBoxSubject
                .SelectMany(EventToObservableHelper.ToThrottledObservable)
                .Subscribe(textChanges.OnNext);

            SetPath = ReactiveCommand.Create<string>(textChanges.OnNext);

            textChanges
                .WhereNotNull()
                .DistinctUntilChanged()
                .CombineLatest(applyTemplateSubject, (a, b) => a)
                .WithLatestFrom(textBoxSubject.StartWith(default(TextBox)))
                .SubscribeOnDispatcher()
                .Subscribe(a =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var (path, textBox) = a;
                        OnTextChange(path, textBox);
                    });
                });
        }

        public override void OnApplyTemplate()
        {
            ButtonOne = GetTemplateChild("ButtonOne") as Button ?? throw new NullReferenceException("ButtonOne is null");
            ContentControlOne = GetTemplateChild("ContentControlOne") as ContentControl ?? throw new NullReferenceException("ContentControlOne is null");
            TextBoxOne = (GetTemplateChild("StackPanelOne") as FrameworkElement)?.Resources["TextBoxOne"] as TextBox ?? throw new NullReferenceException("StackPanelOne is null");
            TextBoxContent = TextBoxContent ??= TextBoxOne;
            LabelOne = GetTemplateChild("LabelOne") as Label ?? throw new NullReferenceException("LabelOne is null");
            LabelOne.Content = Label;
            base.OnApplyTemplate();
            applyTemplateSubject.OnNext(Unit.Default);
        }

        protected virtual void OnTextChange(string path, TextBox sender)
        {
            var textBox = sender ?? throw new NullReferenceException("TextBoxContent is null");
            textBox.Text = path;
            textBox.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            textBox.Select(path.Length - length, length);
            textBox.ToolTip = path;
            RaiseTextChangeEvent(path);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public object TextBoxContent
        {
            get => GetValue(TextBoxContentProperty);
            set => SetValue(TextBoxContentProperty, value);
        }

        public ICommand SetPath
        {
            get => (ICommand)GetValue(SetPathProperty);
            set => SetValue(SetPathProperty, value);
        }

        public event TextChangeRoutedEventHandler TextChange
        {
            add => AddHandler(TextChangeEvent, value);
            remove => RemoveHandler(TextChangeEvent, value);
        }

        protected abstract (bool? result, string path) OpenDialog(string filter, string extension);

        private static void TextBoxContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PathBrowser ?? throw new NullReferenceException("PathBrowser is null")).textBoxContentChanges.OnNext(e.NewValue);
        }

        protected void RaiseTextChangeEvent(string text)
        {
            Dispatcher.Invoke(() => RaiseEvent(new TextRoutedEventArgs(TextChangeEvent, text)));
        }

        public class TextRoutedEventArgs : RoutedEventArgs
        {
            public TextRoutedEventArgs(RoutedEvent routedEvent, string text) : base(routedEvent)
            {
                Text = text;
            }

            public string Text { get; set; }
        }

        public delegate void TextChangeRoutedEventHandler(object sender, TextRoutedEventArgs e);
    }
}