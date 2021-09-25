using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Fasterflect;

namespace UtilityWpf.Controls
{

    public class ButtonTextControl : Control
    {
        static ButtonTextControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonTextControl), new FrameworkPropertyMetadata(typeof(ButtonTextControl)));
        }
    }

    public class ButtonsControl : ListBox<ButtonTextControl>
    {
        public static readonly DependencyProperty CommandPathProperty =
            DependencyProperty.Register("CommandPath", typeof(string), typeof(ButtonsControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ButtonsControl));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ButtonsControl), new PropertyMetadata(Orientation.Vertical, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static ButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonsControl), new FrameworkPropertyMetadata(typeof(ButtonsControl)));
        }

        #region properties
        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }
        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        #endregion properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is ButtonTextControl buttonText)
            {
                _ = buttonText.ApplyTemplate();

                if (buttonText.ChildOfType<Button>() is Button button)
                    if (string.IsNullOrEmpty(CommandPath) == false)                 
                        button.Command = (System.Windows.Input.ICommand)item.TryGetValue(CommandPath);

                if (string.IsNullOrEmpty(DisplayMemberPath) == false)
                    if (button.Content is TextBlock textBlock)
                        BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, CreateBinding());
            }

            base.PrepareContainerForItemOverride(element, item);


            Binding CreateBinding()
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.OneWay,
                };
            }

            //Binding CreateCommandBinding()
            //{
            //    return new Binding
            //    {
            //        Source = item,
            //        Path = new PropertyPath(CommandPath),
            //        Mode = BindingMode.OneTime,
            //    };
            //}
        }


    }
}
