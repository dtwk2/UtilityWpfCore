﻿using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace UtilityWpf.Behavior
{
    /// <summary>
    /// <a href="https://social.technet.microsoft.com/wiki/contents/articles/46543.wpf-handling-both-click-and-doubleclick-events.aspx"></a>
    /// </summary>
    /// <remarks>
    /// <a href="https://stackoverflow.com/questions/12669756/mousedown-doesnt-work-in-grid-only-on-buttons-which-in-grids"></a>
    /// Setting IsHitTestVisible alone will not make it work.
    /// Elements are not clickable if Background is set to None.
    /// To make it clickable(applies to grid, stackpanel, etc) set the Background to #00000000.
    /// This is more like a workaround however it works and looks fine.
    /// </remarks>
    public class ClickBehavior : Behavior<UIElement>
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public ClickBehavior()
        {
            _timer.Interval = TimeSpan.FromSeconds(0.2);
            _timer.Tick += Timer_Tick;
        }

        public static readonly DependencyProperty ClickCommandPropery =
             DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(ClickBehavior));

        public ICommand ClickCommand
        {
            get => (ICommand)GetValue(ClickCommandPropery);
            set => SetValue(ClickCommandPropery, value);
        }

        public static readonly DependencyProperty DoubleClickCommandPropery =
            DependencyProperty.Register(nameof(DoubleClickCommand), typeof(ICommand), typeof(ClickBehavior));

        public ICommand DoubleClickCommand
        {
            get => (ICommand)GetValue(DoubleClickCommandPropery);
            set => SetValue(DoubleClickCommandPropery, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject is Control control)
            {
                control.Loaded += AssociatedObject_Loaded;
                control.Unloaded += AssociatedObject_Unloaded;
            }
            else
            {
                AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _timer.Tick -= Timer_Tick;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Control)!.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as Control)!.Unloaded -= AssociatedObject_Unloaded;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
            ClickCommand?.Execute(null);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _timer.Stop();
                DoubleClickCommand?.Execute(null);
            }
            else
            {
                _timer.Start();
            }
        }
    }
}