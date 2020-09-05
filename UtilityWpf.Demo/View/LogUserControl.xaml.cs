using ReactiveUI;
using Splat;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for LogUserControl.xaml
    /// </summary>
    public partial class LogUserControl : UserControl
    {
        public LogUserControl()
        {
            InitializeComponent();
        }
    }

    internal class LogViewModel : ReactiveObject, IEnableLogger
    {
        private string species;

        public LogViewModel()
        {
            this.LogCommand = new Command.RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(this.Species))
                {
                    this
                        .Log()
                        .Error("No species was specified!");
                }
                this
                    .Log()
                    .Info("Species {0} was discovered at {1}.", this.Species, "Home");
            });
        }

        public Command.RelayCommand LogCommand { get; }

        public string Species
        {
            get => this.species;
            set => this.RaiseAndSetIfChanged(ref this.species, value);
        }
    }
}