using System;
using System.Linq;
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
            this.Content = new GroupsControl
            {
                DisplayMemberPath = this.DisplayMemberPath,
                ItemsSource = this.ItemsSource,
                IsReadOnlyPath = this.IsReadOnlyPath
            };


            this.WhenAnyValue(a => a.ItemsSource)
                .Skip(1)
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.IsReadOnlyPath))
                     .Subscribe(a =>
                     {
                         var (itemsSource, memberPath, readOnlyPath) = a;

                         this.Dispatcher.InvokeAsync(() =>
                         {
                             if (this.Content is GroupsControl msn)
                             {
                                 msn.ItemsSource = itemsSource;
                                 msn.DisplayMemberPath = memberPath;
                                 msn.IsReadOnlyPath = readOnlyPath;
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
}
