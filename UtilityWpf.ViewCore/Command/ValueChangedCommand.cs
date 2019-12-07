using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace UtilityWpf.View
{
    public class ValueChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        protected event Action<KeyValuePair<string, double>> Event;

        public void Execute(object parameter)
        {
            var kvp = (parameter as View.SliderItemsControl.KeyValuePairRoutedEventArgs).KeyValuePair;

            Event.Invoke(kvp);
        }
    }
}