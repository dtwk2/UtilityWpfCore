using DateWork.Models;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Controls.Date
{
    public class CalendarControl : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(CalendarControl));
        public static readonly DependencyProperty SelectedItemProperty = ListBox.SelectedItemProperty.AddOwner(typeof(CalendarControl));
        public static readonly RoutedEvent SelectionChangedEvent = ListBox.SelectionChangedEvent.AddOwner(typeof(CalendarControl));
        public static readonly DependencyProperty ValueConverterProperty = MonthGridControl.ValueConverterProperty.AddOwner(typeof(CalendarControl), new FrameworkPropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CalendarControl { monthGridControl: { } md } control)
            {
                md.ValueConverter = (IValueConverter)e.NewValue;
            }
        }

        static CalendarControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarControl), new FrameworkPropertyMetadata(typeof(CalendarControl)));
        }

        DateModel dateModel = new();
        private MonthGridControl? monthGridControl;

        public CalendarControl()
        {

        }

        public override void OnApplyTemplate()
        {
            monthGridControl = this.GetTemplateChild("MonthGridControl") as MonthGridControl;
            var datePickerControl = this.GetTemplateChild("DatePickerControl") as DatePickerControl;

            this.SelectedItem = monthGridControl.SelectedItem;
            datePickerControl.DateChange += DatePickerControl_DateChange;
            monthGridControl.SelectionChanged += MonthGridControl_SelectionChanged;
            monthGridControl.ValueConverter = this.ValueConverter;
            datePickerControl.Month = dateModel.Month;
            datePickerControl.Year = dateModel.Year;
            monthGridControl.ItemsSource = dateModel.Days;
            monthGridControl.SelectedItem = dateModel.Current;
            base.OnApplyTemplate();

            void DatePickerControl_DateChange(object sender, DateChangeEventArgs e)
            {
                dateModel.Month = e.Month;
                dateModel.Year = e.Year;
                monthGridControl.ItemsSource = dateModel.Days;
                monthGridControl.SelectedItem = dateModel.Current;
            }

            void MonthGridControl_SelectionChanged(object sender, RoutedEventArgs e)
            {
                if (sender is MonthGridControl listBox)
                {
                    this.SelectedItem = listBox.SelectedItem;
                }
            }

        }

        public IValueConverter ValueConverter
        {
            get { return (IValueConverter)GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }
    }
}
