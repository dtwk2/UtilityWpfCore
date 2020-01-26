using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf
{
    public class OutputCommand : Control, ICommand
    {
        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(OutputCommand), new PropertyMetadata(null));

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.Dispatcher.InvokeAsync(() => Output = parameter, System.Windows.Threading.DispatcherPriority.DataBind);
        }
    }

    //public class OutputCommand2 : Control, ICommand
    //{
    //    public IEnumerable Output
    //    {
    //        get { return (IEnumerable)GetValue(OutputProperty); }
    //        set { SetValue(OutputProperty, value); }
    //    }

    //    public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(IEnumerable), typeof(OutputCommand2), new PropertyMetadata(null));

    //    public event EventHandler CanExecuteChanged;

    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        this.Dispatcher.InvokeAsync(() => Output = parameter as IEnumerable, System.Windows.Threading.DispatcherPriority.Background);
    //    }
    //}
}