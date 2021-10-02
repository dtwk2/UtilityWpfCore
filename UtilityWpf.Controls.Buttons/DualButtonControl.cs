using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Controls.Buttons
{
    public class DualButtonControl : SwitchControl
    {
        static DualButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DualButtonControl), new FrameworkPropertyMetadata(typeof(DualButtonControl)));
        }

        public override void OnApplyTemplate()
        {
            var edit2Button = GetTemplateChild("Edit2Button") as ButtonBase ?? throw new NullReferenceException("Deserialized object is null");
            edit2Button.Click += EditButton_OnClick;
            base.OnApplyTemplate();
        }

        protected override void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            SetValueCommand.Execute(!Value ? Main : Alternate);
        }
    }
}