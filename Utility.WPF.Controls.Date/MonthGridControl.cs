using ReactiveUI;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Controls.Date
{

    /// <summary>
    /// Override <see cref="PrepareContainerForItemOverride(DependencyObject, object)"/>
    /// and sets the content of the <see cref="DayControl"/> with <see cref="ValueConverter"/>
    /// </summary>
    public class ListBox2 : ListBox
    {
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(ListBox2), new PropertyMetadata());

        protected sealed override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not ListBoxItem t)
                throw new System.Exception("s fdsd  77ffs");
            t.ApplyTemplate();
            var dayControl = t.ChildOfType<DayControl>();
            _ = this.WhenAnyValue(a => a.ValueConverter)
                .WhereNotNull()
                .Subscribe(a => dayControl.Content = a.Convert(item, null, null, null));
            base.PrepareContainerForItemOverride(element, item);
        }

        public IValueConverter ValueConverter
        {
            get { return (IValueConverter)GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }
    }

    static class VisualTreeHelperEx
    {
        /// <summary>
        /// Gets child of specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static T? ChildOfType<T>(this DependencyObject? depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = child as T ?? ChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }

    public class MonthGridControl : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(MonthGridControl), new PropertyMetadata(Changed));
        public static readonly DependencyProperty SelectedItemProperty = ListBox.SelectedItemProperty.AddOwner(typeof(MonthGridControl), new PropertyMetadata(Changed));
        public static readonly DependencyProperty ValueConverterProperty = DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(MonthGridControl), new PropertyMetadata(Changed));
        public static readonly RoutedEvent SelectionChangedEvent = ListBox.SelectionChangedEvent.AddOwner(typeof(MonthGridControl));

        private ListBox2? listBox;

        static MonthGridControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthGridControl), new FrameworkPropertyMetadata(typeof(MonthGridControl)));
        }

        public override void OnApplyTemplate()
        {
            listBox = this.GetTemplateChild("CalendarDaysListBox") as ListBox2;
            listBox.ValueConverter = ValueConverter;
            listBox.ItemsSource = this.ItemsSource;
            listBox.SelectedItem = this.SelectedItem;
            listBox.SelectionChanged += ListBox_SelectionChanged;
            base.OnApplyTemplate();
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MonthGridControl { listBox: { } listBox, } control)
            {
                listBox.ValueConverter = control.ValueConverter;
                listBox.ItemsSource = control.ItemsSource;
                listBox.SelectedItem = control.SelectedItem;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                this.SelectedItem = listBox.SelectedItem;
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