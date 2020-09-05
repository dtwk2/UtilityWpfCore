using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.View
{
    public class DatesList : ListBox
    {
        static DatesList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatesList), new FrameworkPropertyMetadata(typeof(DatesList)));
        }

        public object DatesChangeCommand
        {
            get { return (object)GetValue(DatesChangeCommandProperty); }
            set { SetValue(DatesChangeCommandProperty, value); }
        }

        public static readonly DependencyProperty DatesChangeCommandProperty = DependencyProperty.Register("DatesChangeCommand", typeof(object), typeof(DatesList), new PropertyMetadata(null));

        public DatesList()
        {
            DatesChangeCommand = new DatesChangeCommand(this);
        }
    }

    public class DatesChangeCommand : ICommand
    {
        private readonly DatesList datesList;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public DatesChangeCommand(DatesList datesList)
        {
            this.datesList = datesList;
        }

        public void Execute(object parameter)
        {
            datesList.ItemsSource = parameter as IEnumerable;
        }
    }

    public class DateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate YesterdayTemplate { get; set; }

        public DataTemplate TodayTemplate { get; set; }

        public DataTemplate TomorrowTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //FrameworkElement element = container as FrameworkElement;

            if (item != null && item is DateTime dateTime)
            {
                if (dateTime.IsYesterday())
                    return YesterdayTemplate;
                else if (dateTime.IsToday())
                    return TodayTemplate;
                else if (dateTime.IsTomorrow())
                    return TomorrowTemplate;
                else
                    return DefaultTemplate;
            }

            return null;
        }
    }

    internal static class DateTimeHelper
    {
        public static bool IsYesterday(this DateTime dt) => (dt >= DateTime.Today.AddDays(-1) && dt < DateTime.Today);

        public static bool IsTomorrow(this DateTime dt) => (dt <= DateTime.Today.AddDays(2) && dt > DateTime.Today.AddDays(1));

        public static bool IsToday(this DateTime dt) => DateTime.Today == dt.Date;
    }
}