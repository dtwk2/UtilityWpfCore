using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Dragablz;
using ReactiveUI;

namespace UtilityWpf.Controls
{
    public class MasterNotesControl : MasterBindableControl
    {
        static MasterNotesControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesControl), new FrameworkPropertyMetadata(typeof(MasterNotesControl)));
        }

        public MasterNotesControl()
        {
            Orientation = Orientation.Vertical;
        }

        public override void OnApplyTemplate()
        {
            this.Content = new MasterNotesItemsControl
            {
                DisplayMemberPath = this.DisplayMemberPath,
                ItemsSource = this.ItemsSource
            };

            this.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath))
                .Subscribe(a =>
                {

                    this.Dispatcher.InvokeAsync(() =>
                    {
                        if (Content == null)
                            this.Content = new MasterNotesItemsControl
                            {
                                DisplayMemberPath = this.DisplayMemberPath,
                                ItemsSource = this.ItemsSource
                            };
                        else
                        {
                            if (this.Content is MasterNotesItemsControl msn)
                            {
                                msn.ItemsSource = this.ItemsSource;
                                msn.DisplayMemberPath = this.DisplayMemberPath;
                            }
                            else
                            {
                                throw new ApplicationException("Expected Content to be MasterNotesItemsControl");
                            }
                        }

                  //DoubleAnimation oLabelAngleAnimation    = new DoubleAnimation();
                  //oLabelAngleAnimation.From = 0;
                  //oLabelAngleAnimation.To = this?.ActualHeight??0;
                  //oLabelAngleAnimation.Duration                    = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                  //oLabelAngleAnimation.RepeatBehavior = new RepeatBehavior(4);
                  //this.BeginAnimation(MasterBindableControl.HeightProperty,                    oLabelAngleAnimation);

              }, System.Windows.Threading.DispatcherPriority.Background);
                });

            base.OnApplyTemplate();
        }
    }

    public class MasterNotesItemsControl : DragablzVerticalItemsControl
    {
        static MasterNotesItemsControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesItemsControl), new FrameworkPropertyMetadata(typeof(MasterNotesItemsControl)));
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();
            if (element.ChildOfType<TextBox>() is not TextBox textBox)
                return;

            BindingOperations.SetBinding(textBox, TextBox.TextProperty, CreateBinding(item));

            textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
            textBox.GotFocus += TextBox_GotFocus;
            base.PrepareContainerForItemOverride(element, item);

            Binding CreateBinding(object item)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
            }
        }

    

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void TextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }
    }
}
