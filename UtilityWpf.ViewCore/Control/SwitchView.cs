using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class SwitchView : Control
    {
        // Using a DependencyProperty as the backing store for UnCheckedContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnCheckedButtonContentProperty =
            DependencyProperty.Register("UnCheckedButtonContent", typeof(object), typeof(SwitchView), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for UnCheckedContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnCheckedContentProperty =
            DependencyProperty.Register("UnCheckedContent", typeof(object), typeof(SwitchView), new PropertyMetadata(null));


        // Using a DependencyProperty as the backing store for UnCheckedContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register("CheckedContent", typeof(object), typeof(SwitchView), new PropertyMetadata(null));



        // Using a DependencyProperty as the backing store for UnCheckedContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedButtonContentProperty =
            DependencyProperty.Register("CheckedButtonContent", typeof(object), typeof(SwitchView), new PropertyMetadata(null));


        static SwitchView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchView), new FrameworkPropertyMetadata(typeof(SwitchView)));
        }

        public object CheckedButtonContent
        {
            get { return (object)GetValue(CheckedButtonContentProperty); }
            set { SetValue(CheckedButtonContentProperty, value); }
        }

        public object UnCheckedButtonContent
        {
            get { return (object)GetValue(UnCheckedButtonContentProperty); }
            set { SetValue(UnCheckedButtonContentProperty, value); }
        }

        public object CheckedContent
        {
            get { return (object)GetValue(CheckedContentProperty); }
            set { SetValue(CheckedContentProperty, value); }
        }
        public object UnCheckedContent
        {
            get { return (object)GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }


    }
}
