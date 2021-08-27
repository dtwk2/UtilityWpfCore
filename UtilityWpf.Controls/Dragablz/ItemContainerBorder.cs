using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls.Dragablz
{
    public class ItemContainerBorder : ContentControl
    {
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), typeof(ItemContainerBorder), new PropertyMetadata(false));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ItemContainerBorder), new PropertyMetadata(false));
        public static readonly DependencyProperty IsSelectableProperty =      DependencyProperty.Register("IsSelectable", typeof(bool), typeof(ItemContainerBorder), new PropertyMetadata(false));
       
        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
  


        static ItemContainerBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemContainerBorder), new FrameworkPropertyMetadata(typeof(ItemContainerBorder)));
        }

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    foreach(var child in (this.Content as FrameworkElement)
        //}

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

    }
}
