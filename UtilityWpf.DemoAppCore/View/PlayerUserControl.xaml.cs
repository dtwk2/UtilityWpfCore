using System;
using System.Windows.Controls;
using System.Windows.Threading;
using UtilityEnum;
using UtilityWpf.Property;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for PlayerUserControl.xaml
    /// </summary>
    public partial class PlayerUserControl : UserControl
    {
        public PlayerUserControl()
        {
            InitializeComponent();
            this.DataContext = new PlayerViewModel();
        }

        internal class PlayerViewModel : NPC
        {
            private ProcessState processState;

            public int Value { get; private set; }

            public UtilityEnum.ProcessState ProcessState
            {
                set
                {
                    processState = value;
                    switch (value)
                    {
                        case (ProcessState.Ready):
                            break;

                        case (ProcessState.Terminated):
                            dispatcherTimer.Stop();
                            Value = 0;
                            OnPropertyChanged(nameof(Value));
                            i = 0;

                            break;

                        case (ProcessState.Running):
                            dispatcherTimer.Start();
                            break;

                        case (ProcessState.Blocked):
                            dispatcherTimer.Stop();
                            break;
                    }

                    OnPropertyChanged(nameof(ProcessState));
                }
            }

            private DispatcherTimer dispatcherTimer;

            public PlayerViewModel()
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            }

            private int i = 0;

            private void dispatcherTimer_Tick(object sender, EventArgs e)
            {
                Value = i * 10;
                OnPropertyChanged(nameof(Value));
                i++;
                if (Value == 100)
                    dispatcherTimer.Stop();
            }
        }
    }
}