using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityInterface.Generic;

namespace UtilityWpf.View
{
    public class InputOutputControl<T, R> : Control /*: HeaderBodyControl*/
    {
        //static HeaderBodyControl()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderBodyControl), new FrameworkPropertyMetadata(typeof(HeaderBodyControl)));
        //}

        //public FrameworkElement Body
        //{
        //    get { return (FrameworkElement)GetValue(BodyProperty); }
        //    set { SetValue(BodyProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Body.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty BodyProperty =
        //    DependencyProperty.Register("Body", typeof(FrameworkElement), typeof(HeaderBodyControl), new UIPropertyMetadata(null, Change));

        //private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //}

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input", typeof(T), typeof(InputOutputControl<T, R>), new PropertyMetadata(null, InputChanged));

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(R), typeof(InputOutputControl<T, R>), new PropertyMetadata(null));

        public R Output
        {
            get { return (R)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public T Input
        {
            get { return (T)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        private static void InputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((d as InputOutputControl<T, R>).InputChanges as ISubject<T>).OnNext((T)e.NewValue);
        }

        protected IObservable<T> InputChanges = new Subject<T>();
        //protected ISubject<R> OutputChanges = new Subject<R>();

        static InputOutputControl()
        {
            //  HeaderedContentControl.HeaderProperty.OverrideMetadata(typeof(InputOutputControl<T, R>), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, InputChanged));
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(InputOutputControl<T, R>), new FrameworkPropertyMetadata(typeof(ListBoxEx)));
        }

        public InputOutputControl(IFunction<T, R> service, Func<IObservable<T>, IObservable<T>> func = null)
        {
            if (func != null)
                func(InputChanges).Subscribe(_ => (InputChanges as ISubject<T>).OnNext(_));
            Init(service);
        }

        public InputOutputControl()
        {
            // used for xaml
        }

        protected virtual void Init(IFunction<T, R> service)
        {
            InputChanges.Subscribe(_ =>
            {
                this.Dispatcher.InvokeAsync(() =>
               {
                   this.SetValue(OutputProperty, service.Function(_));
               }, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
            });
        }
    }
}