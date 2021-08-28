using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Dragablz;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Media.Animation;

namespace UtilityWpf.Controls
{
    public class MasterGroupsControl : MasterBindableControl
    {
        static MasterGroupsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsControl)));
        }

        public MasterGroupsControl()
        {
        }

        public override void OnApplyTemplate()
        {
            this.Content = new MasterGroupsItemsControl
            {
                DisplayMemberPath = this.DisplayMemberPath,
                ItemsSource = this.ItemsSource,

            };


            this.WhenAnyValue(a => a.ItemsSource)
                .Skip(1)
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath))
                     .Subscribe(a =>
                     {
                         this.Dispatcher.InvokeAsync(() =>
                         {
                             if (this.Content is MasterGroupsItemsControl msn)
                             {
                                 msn.ItemsSource = this.ItemsSource;
                                 msn.DisplayMemberPath = this.DisplayMemberPath;
                             }
                             else
                             {
                                 throw new ApplicationException("Expected Content to be MasterGroupsControl");
                             }


                             DoubleAnimation oLabelAngleAnimation = new DoubleAnimation();
                             oLabelAngleAnimation.From = 0;
                             oLabelAngleAnimation.To = this?.ActualHeight ?? 0;
                             oLabelAngleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                             this.BeginAnimation(MasterBindableControl.HeightProperty, oLabelAngleAnimation);
                             Visibility = Visibility.Visible;

                         }, System.Windows.Threading.DispatcherPriority.Background);

                     });

            base.OnApplyTemplate();
        }
    }

    public class MasterGroupsItemsControl : DragablzVerticalItemsControl
    {
        static MasterGroupsItemsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsItemsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsItemsControl)));
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is Control control)
            {
                SetBinding(control, item);
            }
            base.PrepareContainerForItemOverride(element, item);
        }


        private void SetBinding(Control element, object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;
            _ = element.ApplyTemplate();
            if (element.ChildOfType<TextBlock>() is not TextBlock textBox)
                return;
      
            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(textBox, TextBlock.TextProperty, myBinding);
        }
    }
}
