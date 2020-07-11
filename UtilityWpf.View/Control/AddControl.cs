using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.View
{
    public class AddControl : ContentControl
    {

        static AddControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AddControl), new FrameworkPropertyMetadata(typeof(AddControl)));

        }

        public AddControl()
        {
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(AddControl), new PropertyMetadata(null));


        public override void OnApplyTemplate()
        {
            var button = this.GetTemplateChild("Button1") as Button;
            button.Click += Button_Click;
            base.OnApplyTemplate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Execute(CommandParameter);
        }

        //public bool CanExecute(object parameter)
        //{
        //    return true;
        //}

        public virtual void Execute(object parameter)
        {

        }
    }
}
