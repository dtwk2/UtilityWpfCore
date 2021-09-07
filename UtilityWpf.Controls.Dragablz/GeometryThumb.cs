﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UtilityWpf.Controls.Dragablz
{
    public class GeometryThumb : Thumb
    {
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(GeometryThumb), new PropertyMetadata(null));

        static GeometryThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryThumb), new FrameworkPropertyMetadata(typeof(GeometryThumb)));
        }

        public GeometryThumb()
        {
            //Foreground = Brushes.DarkGray;
        }

        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }



        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(GeometryThumb), new PropertyMetadata(Brushes.WhiteSmoke));




    }
}
