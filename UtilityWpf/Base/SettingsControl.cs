using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Behavior;
using Selector = System.Windows.Controls.Primitives.Selector;
namespace UtilityWpf.Base
{
    public class SettingsControl : Control
    {
             public static readonly DependencyProperty SelectedDockProperty =
            DependencyProperty.Register("SelectedDock", typeof(Dock?), typeof(SettingsControl), new PropertyMetadata(null, Change /*m,coerceValueCallback: Callback*/));


        static SettingsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsControl), new FrameworkPropertyMetadata(typeof(SettingsControl)));
        }

        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            var behavior = this.GetTemplateChild("MenuItemButtonGroupBehavior") as MenuItemButtonGroupBehavior;
            behavior.WhenAnyValue(a => a.SelectedItem)
                .Subscribe(a =>
                {
                    if (a is null)
                        return;
                    if (a is EnumMember { Value: null })
                    {
                        SelectedDock = null;
                        return;
                    }
                    if (a is EnumMember { Value: Dock value })
                    {
                        SelectedDock = value;
                        return;
                    }
          
                    throw new Exception("fd   99f");
                });
            base.OnApplyTemplate();
        }
        //private static object Callback(DependencyObject d, object baseValue)
        //{
        //    if (baseValue is EnumMember { Value: var value } member)
        //    {
        //        return value;
        //    }
        //    throw new Exception("DGF£FGVV vv446");
        //}

        public Dock? SelectedDock
        {
            get { return (Dock?)GetValue(SelectedDockProperty); }
            set { SetValue(SelectedDockProperty, value); }
        }
    }
}
