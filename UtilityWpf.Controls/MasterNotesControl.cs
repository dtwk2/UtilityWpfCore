using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using Evan.Wpf;

namespace UtilityWpf.Controls
{
    public class MasterNotesControl : MasterControl
    {
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyHelper.Register<string>();
        public static readonly DependencyProperty ItemsSourceProperty = DependencyHelper.Register<IEnumerable>();

        static MasterNotesControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesControl), new FrameworkPropertyMetadata(typeof(MasterNotesControl)));
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
    }

    public class MasterNotesItemsControl : DragablzVerticalItemsControl
    {
        //protected override void AddChild(object value)
        //{
        //    base.AddChild(value);
        //}

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            control.ApplyTemplate();
            if (element.ChildOfType<TextBox>() is not TextBox textBox)
                return;

            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };   
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, myBinding);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
