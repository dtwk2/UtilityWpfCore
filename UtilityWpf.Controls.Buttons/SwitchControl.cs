﻿using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UtilityWpf.Controls.Buttons
{
    public class SwitchControl : Control
    {
        private readonly ReactiveCommand<object, object> setValueCommand;
        protected ButtonBase EditButton;

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register("ToolTipText", typeof(string), typeof(SwitchControl), new PropertyMetadata(null));

        public static readonly DependencyProperty MainProperty =
            DependencyProperty.Register("Main", typeof(object), typeof(SwitchControl), new PropertyMetadata("Yes"));

        public static readonly DependencyProperty AlternateProperty =
            DependencyProperty.Register("Alternate", typeof(object), typeof(SwitchControl), new PropertyMetadata("No"));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(SwitchControl), new PropertyMetadata(true, null));

        public static readonly DependencyProperty KeyValueProperty = DependencyProperty.Register("KeyValue", typeof(object), typeof(SwitchControl));

        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register("ButtonWidth", typeof(double), typeof(SwitchControl), new PropertyMetadata(120d));

        public static readonly RoutedEvent ButtonToggleEvent = EventManager.RegisterRoutedEvent("ButtonToggle", RoutingStrategy.Bubble, typeof(ToggleEventHandler), typeof(SwitchControl));

        public delegate void ToggleEventHandler(object sender, ToggleEventArgs size);

        static SwitchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchControl), new FrameworkPropertyMetadata(typeof(SwitchControl)));
        }

        public SwitchControl()
        {
            setValueCommand = ReactiveCommand.Create<object, object>(a => a);
            //KeyValue = ValueToKey();
        }

        public override void OnApplyTemplate()
        {
            EditButton = GetTemplateChild("EditButton") as ButtonBase;

            setValueCommand.DistinctUntilChanged().Subscribe(a =>
            {
                Value = Main.Equals(a);
                RaiseToggleEvent(Value ? Main : Alternate, Value);
            });
            base.OnApplyTemplate();
        }

        protected virtual void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            //if (EditButton is ToggleButton toggle)
            RaiseToggleEvent(ValueToKey(), this.Value);
        }

        public virtual object ValueToKey()
        {
            return Value ? Main : Alternate;
        }

        public virtual bool KeyToValue(object key)
        {
            return key.Equals(Main) || (key.Equals(Alternate) ? false : throw new Exception("SD£VVvvvv"));
        }

        public event ToggleEventHandler ButtonToggle
        {
            add => AddHandler(ButtonToggleEvent, value);
            remove => RemoveHandler(ButtonToggleEvent, value);
        }

        protected void RaiseToggleEvent(object key, bool? isChecked)
        {
            RoutedEventArgs newEventArgs = new ToggleEventArgs(ButtonToggleEvent, key, isChecked);
            RaiseEvent(newEventArgs);
        }

        #region properties

        public object Main
        {
            get => GetValue(MainProperty);
            set => SetValue(MainProperty, value);
        }

        public object Alternate
        {
            get => GetValue(AlternateProperty);
            set => SetValue(AlternateProperty, value);
        }

        public string ToolTipText
        {
            get => (string)GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        public bool Value
        {
            get => (bool)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        //public object KeyValue
        //{
        //    get => (object)GetValue(KeyValueProperty);
        //    set => SetValue(KeyValueProperty, value);
        //}

        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        #endregion properties

        public ICommand SetValueCommand => setValueCommand;

        public class ToggleEventArgs : RoutedEventArgs
        {
            public ToggleEventArgs(RoutedEvent routedEvent, object key, bool? isChecked) : base(routedEvent)
            {
                Key = key;
                IsChecked = isChecked;
            }

            public bool? IsChecked { get; }

            public object Key { get; }
        }
    }
}