﻿using Evan.Wpf;
using PropertyTools.Wpf;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Mixins;

namespace UtilityWpf.Controls.Master
{
    public class DoubleContentControl : HeaderedContentControlx
    {
        public static readonly DependencyProperty PositionProperty = DependencyHelper.Register<Dock>(new PropertyMetadata(Dock.Bottom, Changed));
        private ReplaySubject<Dock> dockSubject = new(1);

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Dock dock)
                d.Dispatcher.InvokeAsync(() => (d as DoubleContentControl).dockSubject.OnNext(dock));
        }

        protected readonly ReplaySubject<WrapPanel> wrapPanelSubject = new(1);
        protected readonly ReplaySubject<DockPanelSplitter> dockPanelSplitterSubject = new(1);
        public static readonly DependencyProperty OrientationProperty = DependencyHelper.Register<Orientation>();

        static DoubleContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleContentControl), new FrameworkPropertyMetadata(typeof(DoubleContentControl)));
        }

        public DoubleContentControl()
        {
            this.Control<WrapPanel>().Subscribe(wrapPanelSubject);

            wrapPanelSubject
                .Take(1)
                .CombineLatest(this.Observable<Orientation>().DistinctUntilChanged())
                .Subscribe(a =>
                {
                    a.First.Orientation = a.Second;
                });

            dockSubject
                .CombineLatest(wrapPanelSubject, dockPanelSplitterSubject)
                .Where(a => a.Second != null /*&& a.Third != null*/)
                .Subscribe(c =>
                {
                    var (Position, wrapPanel, splitter) = c;

                    DockPanel.SetDock(wrapPanel, Position);
                    if (splitter != null)
                        DockPanel.SetDock(splitter, Position);

                    if (Position == Dock.Right)
                    {
                        wrapPanel.Orientation = Orientation.Vertical;
                    }
                    else if (Position == Dock.Bottom)
                    {
                        wrapPanel.Orientation = Orientation.Horizontal;
                    }
                    else if (Position == Dock.Left)
                    {
                        wrapPanel.Orientation = Orientation.Vertical;
                    }
                    else if (Position == Dock.Top)
                    {
                        wrapPanel.Orientation = Orientation.Horizontal;
                    }
                });
        }

        #region properties

        public Dock Position
        {
            get { return (Dock)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            if (this.GetTemplateChild("PART_DockPanel") is not DockPanel dockPanel)
                throw new ApplicationException();
            if (this.GetTemplateChild("PART_WrapPanel") is not WrapPanel wrapPanel)
                throw new ApplicationException();

            wrapPanelSubject.OnNext(wrapPanel);

            if (this.GetTemplateChild("PART_DockPanelSplitter") is not DockPanelSplitter dockPanelSplitter)
                throw new ApplicationException();
            dockPanelSplitterSubject.OnNext(dockPanelSplitter);

            base.OnApplyTemplate();
        }
    }
}