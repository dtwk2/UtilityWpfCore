using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class AddControl : ContentControlx
    {
        private ItemsControl? itemsControl;
        public static readonly DependencyProperty OrientationProperty = DependencyHelper.Register<Orientation>(new PropertyMetadata(Orientation.Horizontal));
        public static readonly DependencyProperty CommandParameterProperty = DependencyHelper.Register<object>();
        public static readonly DependencyProperty ItemsSourceProperty = DependencyHelper.Register<IEnumerable>();

        static AddControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AddControl), new FrameworkPropertyMetadata(typeof(AddControl)));
        }

        public AddControl()
        {
            this.WhenAnyValue(a => a.Orientation).CombineLatest(this.SelectControlChanges<WrapPanel>("WrapPanel1"), (a, b) => (a, b))
                .Where(a => a.b != null)
                .Subscribe(c =>
                {
                    var (orientation, dockPanel) = c;
            
                    if (orientation == Orientation.Horizontal)
                    {
                        DockPanel.SetDock(dockPanel, Dock.Right);
                        dockPanel.Orientation = Orientation.Vertical;
                    }
                    else if (orientation == Orientation.Vertical)
                    {
                        DockPanel.SetDock(dockPanel, Dock.Bottom);
                        dockPanel.Orientation = Orientation.Horizontal;
                    }
                });
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            var buttonAdd = this.GetTemplateChild("ButtonPlus") as Button;
            var buttonRemove = this.GetTemplateChild("ButtonMinus") as Button;
            var dockPanel = this.GetTemplateChild("DockPanel1") as DockPanel;
            var wrapPanel = this.GetTemplateChild("WrapPanel1") as WrapPanel;
            //grid = this.GetTemplateChild("Grid1") as Grid;
            buttonAdd.Click += (s, e) => ExecuteAdd(this.GetValue(CommandParameterProperty));
            buttonRemove.Click += (s, e) => ExecuteRemove(this.GetValue(CommandParameterProperty));

            itemsControl = (this.Content as ItemsControl) ?? (this.Content as DependencyObject)?.FindVisualChildren<ItemsControl>().SingleOrDefault();
            if (itemsControl != null)
            {
                this.SetValue(ItemsSourceProperty, itemsControl.ItemsSource);
                wrapPanel.DataContext = itemsControl.ItemsSource;
            }
            base.OnApplyTemplate();
        }



        public virtual void ExecuteAdd(object parameter)
        {

        }

        public virtual void ExecuteRemove(object parameter)
        {

            if (itemsControl != null )
            {
                if (itemsControl.ItemsSource is IList collection && collection.Count > 0)
                    collection.RemoveAt(collection.Count - 1);
                else
                {

                }
            }
            else
            {

            }
        }
    }
}
