using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using UtilityWpf.Controls.Dragablz;
using UtilityWpf.Controls.Master;

namespace UtilityWpf.Controls.Hybrid
{
    public class MasterGroupsControl : MasterBindableControl
    {
        public static readonly DependencyProperty IsReadOnlyPathProperty = DependencyProperty.Register("IsReadOnlyPath", typeof(string), typeof(MasterGroupsControl), new PropertyMetadata(null));

        //static MasterGroupsControl()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterGroupsControl), new FrameworkPropertyMetadata(typeof(MasterGroupsControl)));
        //}

        public MasterGroupsControl()
        {
            ButtonTypes = ButtonType.Add | ButtonType.Remove;
            RemoveOrder = RemoveOrder.Selected;
        }

        public string IsReadOnlyPath
        {
            get => (string)GetValue(IsReadOnlyPathProperty);
            set => SetValue(IsReadOnlyPathProperty, value);
        }

        public override void OnApplyTemplate()
        {
            Content = new GroupsControl
            {
                DisplayMemberPath = DisplayMemberPath,
                ItemsSource = ItemsSource,
                IsReadOnlyPath = IsReadOnlyPath
            };

            this.WhenAnyValue(a => a.ItemsSource)
                .Skip(1)
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.IsReadOnlyPath))
                     .Subscribe(a =>
                     {
                         var (itemsSource, memberPath, readOnlyPath) = a;

                         Dispatcher.InvokeAsync(() =>
                         {
                             if (Content is GroupsControl msn)
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
                             BeginAnimation(HeightProperty, oLabelAngleAnimation);
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
            else if (SelectedItem is { })
            {
                var container = ItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedItem);
                container.SetValue(Attached.Ex.IsReadOnlyProperty, isAdd);
            }
        }
    }
}