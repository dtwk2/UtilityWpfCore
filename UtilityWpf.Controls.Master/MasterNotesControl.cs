using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using UtilityWpf.Controls.Dragablz;

namespace UtilityWpf.Controls.Master
{
    public class MasterNotesControl : MasterBindableControl
    {
        static MasterNotesControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesControl), new FrameworkPropertyMetadata(typeof(MasterNotesControl)));
        }

        public MasterNotesControl()
        {
            Position = Dock.Bottom;
        }

        public override void OnApplyTemplate()
        {
            this.Content = new NotesControl
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
                            this.Content = new NotesControl
                            {
                                DisplayMemberPath = this.DisplayMemberPath,
                                ItemsSource = this.ItemsSource
                            };
                        else
                        {
                            if (this.Content is NotesControl msn)
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
}
