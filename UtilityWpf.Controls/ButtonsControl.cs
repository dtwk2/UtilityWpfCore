using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf.Controls
{
    public class ButtonsControl : ListBox<Button>
    {
        public static readonly DependencyProperty CommandPathProperty =
            DependencyProperty.Register("CommandPath", typeof(string), typeof(ButtonsControl), new PropertyMetadata(null));

        static ButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonsControl), new FrameworkPropertyMetadata(typeof(ButtonsControl)));
        }

        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Button button)
                return;
            _ = button.ApplyTemplate();
            if (string.IsNullOrEmpty(CommandPath) == false)
                BindingOperations.SetBinding(button, Button.CommandProperty, CreateCommandBinding());

            if (element.ChildOfType<Button>()?.Content is not TextBlock textBlock)
                return;
            if (string.IsNullOrEmpty(DisplayMemberPath) == false)
                BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, CreateBinding());

 
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

            Binding CreateCommandBinding()
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(CommandPath),
                    Mode = BindingMode.OneWay,
                };
            }
        }
    }
}
