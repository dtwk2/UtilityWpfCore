using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using UtilityWpf.Command;

namespace UtilityWpf.Controls.FileSystem
{
    public class BrowserCommand : RelayCommand //, IObservable<string>
    {

      //  private readonly ReplaySubject<string> subject = new(1);

        public BrowserCommand(Func<IObservable<string>> func) :base(GetAction(func, out var sub))
        {
            sub.Subscribe(a =>
            {
                RaiseTextChangedEvent(a);
            });
        }

        static Action GetAction(Func<IObservable<string>> func, out ReplaySubject<string> replaySubject)
        {
            replaySubject = new(1);
            ReplaySubject<string> rSubject1 = new(1);
            rSubject1.Subscribe(replaySubject);
            IDisposable? disposable = null;
            return new Action(() =>
            {
                disposable?.Dispose();
                disposable = func().Subscribe(a=>
                rSubject1.OnNext(a));
            });
       
        }

        //protected readonly Subject<string> filterChanges = new Subject<string>();
        //protected readonly Subject<string> extensionChanges = new Subject<string>();
        //protected readonly Subject<object> changes = new Subject<object>();

        //public static readonly DependencyProperty FilterProperty =
        //    DependencyProperty.Register("Filter", typeof(string), typeof(PathBrowser), new PropertyMetadata(null, FilterChanged));

        //public static readonly DependencyProperty ExtensionProperty =
        //    DependencyProperty.Register("Extension", typeof(string), typeof(PathBrowser), new PropertyMetadata(null, ExtensionChanged));

        //public static readonly RoutedEvent TextChangeEvent =
        //    EventManager.RegisterRoutedEvent("TextChange", RoutingStrategy.Bubble, typeof(TextChangedRoutedEventHandler), typeof(PathBrowser));

        protected void RaiseTextChangedEvent(string text)
        {
            this.TextChanged?.Invoke(text);
        }

        //public IDisposable Subscribe(IObserver<string> observer)
        //{
        //    throw new NotImplementedException();
        //}

        public event Action<string> TextChanged;
        //public event EventHandler CanExecuteChanged;

        //public bool CanExecute(object parameter)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Execute(object parameter)
        //{
        //    throw new NotImplementedException();
        //}
        // public event EventHandler CanExecuteChanged;

        //public string Filter
        //{
        //    get => (string)GetValue(FilterProperty);
        //    set => SetValue(FilterProperty, value);
        //}

        //private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Browser)?.filterChanges.OnNext((string)e.NewValue);
        //}

        //public string Extension
        //{
        //    get => (string)GetValue(ExtensionProperty);
        //    set => SetValue(ExtensionProperty, value);
        //}

        //private static void ExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Browser ?? throw new NullReferenceException("FileBrowser object is null")).extensionChanges.OnNext((string)e.NewValue);
        //}
    }
}