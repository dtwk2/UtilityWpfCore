using System;
using System.Windows;
using System.Windows.Controls;
using UtilityEnum;

namespace UtilityWpf.Controls
{
    public class EnumItems : Control
    {
        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register("Enum", typeof(Type), typeof(EnumItems), new PropertyMetadata(typeof(Switch)));

        static EnumItems()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumItems), new FrameworkPropertyMetadata(typeof(EnumItems)));
        }

        public Type Enum
        {
            get { return (Type)GetValue(EnumProperty); }
            set { SetValue(EnumProperty, Enum); }
        }
    }
}