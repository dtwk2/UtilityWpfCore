using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityInterface.Generic;

namespace UtilityWpf.Controls
{
    public class InputOutputControl<T, R> : Control /*: HeaderBodyControl*/
    {
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

        static InputOutputControl()
        {
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