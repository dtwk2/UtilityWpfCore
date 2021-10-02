using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Controls.Dragablz;

namespace UtilityWpf.Controls.Hybrid
{
    public class MasterButtonsControl : MasterBindableControl
    {
        public static readonly DependencyProperty CommandPathProperty =
         DependencyProperty.Register("CommandPath", typeof(string), typeof(MasterButtonsControl), new PropertyMetadata(null));

        static MasterButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterButtonsControl), new FrameworkPropertyMetadata(typeof(MasterButtonsControl)));
        }

        public MasterButtonsControl()
        {
            Position = Dock.Bottom;
        }

        #region properties
        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }
        #endregion properties

        public override void OnApplyTemplate()
        {
            Content = new ButtonsControl
            {
                CommandPath = CommandPath,
                DisplayMemberPath = DisplayMemberPath,
                ItemsSource = ItemsSource
            };

            this.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.CommandPath))
                .Subscribe(tuple =>
                {
                    var (i, d, c) = tuple;
                    Dispatcher.InvokeAsync(() =>
                    {
                        if (Content == null)
                            Content = new ButtonsControl
                            {
                                CommandPath = c,
                                DisplayMemberPath = d,
                                ItemsSource = i
                            };
                        else
                        {
                            if (Content is ButtonsControl msn)
                            {
                                msn.CommandPath = c;
                                msn.ItemsSource = i;
                                msn.DisplayMemberPath = d;
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
}
