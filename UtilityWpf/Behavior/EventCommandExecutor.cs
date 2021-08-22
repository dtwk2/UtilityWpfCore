using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using UtilityWpf.Utility;

namespace UtilityWpf.Behavior
{
    //https://stackoverflow.com/questions/6205472/mvvm-passing-eventargs-as-command-parameter
    // answered Oct 7 '14 at 23:03 pjs
    public class EventCommandExecuter : TriggerAction<DependencyObject>
    {
        #region Constructors

        public EventCommandExecuter()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public EventCommandExecuter(CultureInfo culture)
        {
            Culture = culture;
        }

        #endregion Constructors

        #region Properties

        #region Command

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandExecuter), new PropertyMetadata(null));

        #endregion Command

        #region EventArgsConverterParameter

        public object EventArgsConverterParameter
        {
            get { return (object)GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        public static readonly DependencyProperty EventArgsConverterParameterProperty =
            DependencyProperty.Register("EventArgsConverterParameter", typeof(object), typeof(EventCommandExecuter), new PropertyMetadata(null));

        #endregion EventArgsConverterParameter

        public IValueConverter EventArgsConverter { get; set; }

        public CultureInfo Culture { get; set; }

        #endregion Properties

        protected override void Invoke(object parameter)
        {
            var cmd = Command;

            if (cmd != null)
            {
                var param = parameter;

                if (EventArgsConverter != null)
                {
                    param = EventArgsConverter.Convert(parameter, typeof(object), EventArgsConverterParameter, CultureInfo.InvariantCulture);
                }
                else if (parameter.GetType() is { } type && type.IsGenericType && type.GetGenericTypeDefinition() is { } gType)
                {
                    if (gType == typeof(RoutedEventArgs<>))
                    {
                        param = type.GetProperty(nameof(RoutedEventArgs<object>.Value)).GetValue(parameter);
                    }
                    else if (gType == typeof(RoutedPropertyChangedEventArgs<>))
                    {
                        param = type.GetProperty(nameof(RoutedPropertyChangedEventArgs<object>.NewValue)).GetValue(parameter);
                    }
                }
                if (cmd.CanExecute(param))
                {
                    cmd.Execute(param);
                }
            }
        }
    }
}