using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilityEnum;

namespace UtilityWpf.View
{
    public class TenseSwitch : Control
    {

        static TenseSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TenseSwitch), new FrameworkPropertyMetadata(typeof(TenseSwitch)));
        }

        ToggleSwitch.HorizontalToggleSwitch toggleSwitch;

        public override void OnApplyTemplate()
        {
            toggleSwitch = this.GetTemplateChild("ToggleSwitch") as ToggleSwitch.HorizontalToggleSwitch;
            toggleSwitch.Unchecked += (s, e) => RaiseEvent(Tense.Past);
            toggleSwitch.Checked += (s, e) => RaiseEvent(Tense.Future);
            this.Dispatcher.InvokeAsync(() =>
            {
                if (Tense == Tense.Future)
                    RaiseEvent(Tense.Future);
                else
                    RaiseEvent(Tense.Past);
            });
        }



        public Tense Tense
        {
            get { return (Tense)GetValue(TenseProperty); }
            set { SetValue(TenseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tense.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TenseProperty =
            DependencyProperty.Register("Tense", typeof(Tense), typeof(TenseSwitch), new PropertyMetadata(Tense.Future));




        public static readonly RoutedEvent TenseChangedEvent = EventManager.RegisterRoutedEvent("TenseChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TenseSwitch));

        public event RoutedEventHandler TenseChanged
        {
            add { AddHandler(TenseChangedEvent, value); }
            remove { RemoveHandler(TenseChangedEvent, value); }
        }

        public void RaiseEvent(Tense tense)
        {
            // Raise the routed event "TenseChanged"
            RaiseEvent(new RoutedTenseChangedEventArgs(TenseSwitch.TenseChangedEvent, tense));
        }




    }
    public class RoutedTenseChangedEventArgs : RoutedEventArgs
    {
        public Tense Tense { get; set; }

        public RoutedTenseChangedEventArgs(RoutedEvent routedEvent, Tense tense) : base(routedEvent)
        {
            Tense = tense;
        }

        public RoutedTenseChangedEventArgs() : base(null)
        {
        }
    }
}
