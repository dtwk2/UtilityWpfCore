﻿
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Evan.Wpf;
using ReactiveUI;
using System.Collections.Specialized;
using UtilityWpf.Abstract;
using UtilityWpf.Mixins;
using System.Reactive.Subjects;
using PropertyTools.Wpf;

namespace UtilityWpf.Controls
{


    public class DoubleContentControl : ContentControlx
    {

        public static readonly DependencyProperty PositionProperty = DependencyHelper.Register<Dock>(new PropertyMetadata(Dock.Bottom));
        protected readonly ReplaySubject<WrapPanel> wrapPanelSubject = new(1);
        protected readonly ReplaySubject<DockPanelSplitter> dockPanelSplitterSubject = new(1);

        static DoubleContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleContentControl), new FrameworkPropertyMetadata(typeof(DoubleContentControl)));
        }

        public DoubleContentControl()
        {
            this.Control<WrapPanel>().Subscribe(wrapPanelSubject);

            wrapPanelSubject.Subscribe(a =>
            {

            });

            this.WhenAnyValue(a => a.Position)
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


        #endregion properties

        public override void OnApplyTemplate()
        {
            var dockPanel = this.GetTemplateChild("PART_DockPanel") as DockPanel;
            var wrapPanel = this.GetTemplateChild("PART_WrapPanel") as WrapPanel;
            var dockPanelSplitter = this.GetTemplateChild("PART_DockPanelSplitter") as DockPanelSplitter;

            if (dockPanel == null)
                throw new ApplicationException();
            if (wrapPanel == null)
                throw new ApplicationException();

            wrapPanelSubject.OnNext(wrapPanel);
            dockPanelSplitterSubject.OnNext(dockPanelSplitter);


            base.OnApplyTemplate();
        }
    }
}
