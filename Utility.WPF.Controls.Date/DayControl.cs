﻿using DateWork.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Date
{
    public class DayControl : ContentControl
    {
        static DayControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayControl), new FrameworkPropertyMetadata(typeof(DayControl)));
        }

        #region Current DependencyProperty
        public DateTime Current
        {
            get { return (DateTime)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }
        public static readonly DependencyProperty CurrentProperty =
                DependencyProperty.Register("Current", typeof(DateTime), typeof(DayControl),
                new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnCurrentPropertyChanged)));

        private static void OnCurrentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnCurrentValueChanged();
            }
        }

        protected void OnCurrentValueChanged()
        {
            RefreshBackground();
        }
        #endregion

        #region Day DependencyProperty
        public DateTime Day
        {
            get { return (DateTime)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }
        public static readonly DependencyProperty DayProperty =
                DependencyProperty.Register("Day", typeof(DateTime), typeof(DayControl),
                new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDayPropertyChanged)));

        private static void OnDayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnDayValueChanged();
            }
        }

        protected void OnDayValueChanged()
        {
            RefreshDayName();
            RefreshMonthDayName();
            RefreshBackground();
        }
        #endregion

        #region DayName DependencyProperty
        public string DayName
        {
            get { return (string)GetValue(DayNameProperty); }
            set { SetValue(DayNameProperty, value); }
        }
        public static readonly DependencyProperty DayNameProperty =
                DependencyProperty.Register("DayName", typeof(string), typeof(DayControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDayNamePropertyChanged)));

        private static void OnDayNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl dayControl)
            {
                dayControl.OnDayNameValueChanged();
            }
        }

        protected void OnDayNameValueChanged()
        {

        }

        private void RefreshDayName()
        {
            DayName = Day.Day.ToString();
        }
        #endregion

        #region MonthDayName DependencyProperty
        public string MonthDayName
        {
            get { return (string)GetValue(MonthDayNameProperty); }
            set { SetValue(MonthDayNameProperty, value); }
        }
        public static readonly DependencyProperty MonthDayNameProperty =
                DependencyProperty.Register("MonthDayName", typeof(string), typeof(DayControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnMonthDayNamePropertyChanged)));

        private static void OnMonthDayNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnMonthDayNameValueChanged();
            }
        }

        protected void OnMonthDayNameValueChanged()
        {

        }

        private void RefreshMonthDayName()
        {
            MonthDayName = Day.ToString("MMMM");
        }
        #endregion

        #region NoteText DependencyProperty
        public string NoteText
        {
            get { return (string)GetValue(NoteTextProperty); }
            set { SetValue(NoteTextProperty, value); }
        }
        public static readonly DependencyProperty NoteTextProperty =
                DependencyProperty.Register("NoteText", typeof(string), typeof(DayControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnNoteTextPropertyChanged)));

        private static void OnNoteTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnNoteTextValueChanged();
            }
        }

        protected void OnNoteTextValueChanged()
        {

        }
        #endregion

        #region DayType DependencyProperty
        public DayType DayType
        {
            get { return (DayType)GetValue(DayTypeProperty); }
            set { SetValue(DayTypeProperty, value); }
        }
        public static readonly DependencyProperty DayTypeProperty =
                DependencyProperty.Register("DayType", typeof(DayType), typeof(DayControl),
                new PropertyMetadata(DayType.CurrentMonth, new PropertyChangedCallback(OnDayTypePropertyChanged)));

        private static void OnDayTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DayControl)
            {
                (obj as DayControl).OnDayTypeValueChanged();
            }
        }

        protected void OnDayTypeValueChanged()
        {

        }
        #endregion

        private void RefreshBackground()
        {
            DayType = Day.ToDayType();
        }

    }
}