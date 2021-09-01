using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Media.Animation;
using UtilityWpf.Controls.Dragablz;

namespace UtilityWpf.Controls.Master
{
    public class MasterGroupsControl : MasterBindableControl
    {
        public static readonly DependencyProperty IsReadOnlyPathProperty = DependencyProperty.Register("IsReadOnlyPath", typeof(string), typeof(MasterGroupsControl), new PropertyMetadata(null));

        static MasterGroupsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsControl)));
        }

        public MasterGroupsControl()
        {
        }
        public string IsReadOnlyPath
        {
            get => (string)GetValue(IsReadOnlyPathProperty);
            set => SetValue(IsReadOnlyPathProperty, value);
        }

        public override void OnApplyTemplate()
        {
            this.Content = new MasterGroupsItemsControl
            {
                DisplayMemberPath = this.DisplayMemberPath,
                ItemsSource = this.ItemsSource,
                IsReadOnlyPath = this.IsReadOnlyPath
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
                                 msn.IsReadOnlyPath = this.IsReadOnlyPath;
                             }
                             else
                             {
                                 throw new ApplicationException("Expected Content to be MasterGroupsControl");
                             }


                             DoubleAnimation oLabelAngleAnimation = new DoubleAnimation();
                             oLabelAngleAnimation.From = 0;
                             oLabelAngleAnimation.To = this?.ActualHeight ?? 0;
                             oLabelAngleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                             this.BeginAnimation(HeightProperty, oLabelAngleAnimation);
                             Visibility = Visibility.Visible;

                         }, System.Windows.Threading.DispatcherPriority.Background);

                     });

            base.OnApplyTemplate();
        }

        protected override void ExecuteAdd()
        {
            ExecuteAddRemove(false);
            base.ExecuteAdd();
        }


        protected override void ExecuteRemove()
        {
            ExecuteAddRemove(true);
            base.ExecuteRemove();
        }

        private void ExecuteAddRemove(bool isAdd)
        {
            if (SelectedItem is UIElement elem)
            {
      
                elem.SetValue(Attached.Ex.IsReadOnlyProperty, isAdd);
            }
            else
            {
                var container = this.ItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedItem);
                container.SetValue(Attached.Ex.IsReadOnlyProperty, isAdd);
            }
        }
    }

    public class MasterGroupsItemsControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty IsReadOnlyPathProperty = DependencyProperty.Register("IsReadOnlyPath", typeof(string), typeof(MasterGroupsItemsControl), new PropertyMetadata(null));


        static MasterGroupsItemsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsItemsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsItemsControl)));
        }

        public string IsReadOnlyPath
        {
            get => (string)GetValue(IsReadOnlyPathProperty);
            set => SetValue(IsReadOnlyPathProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is Control control)
            {
                SetBinding(control, item);
                SetIsReadOnlyBinding(control, item);
            }
            base.PrepareContainerForItemOverride(element, item);
        }


        private void SetBinding(Control element, object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;
            _ = element.ApplyTemplate();
            if (element.ChildOfType<TextBlock>() is not TextBlock textBlock)
                return;
      
            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, myBinding);
        }     


        private void SetIsReadOnlyBinding(Control element, object item)
        {
            if (string.IsNullOrEmpty(IsReadOnlyPath))
                return;

            Binding myBinding = new Binding
            {
                Source = item,
                Path = new PropertyPath(IsReadOnlyPath),
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(element, Attached.Ex.IsReadOnlyProperty, myBinding);
        }
    }
}
