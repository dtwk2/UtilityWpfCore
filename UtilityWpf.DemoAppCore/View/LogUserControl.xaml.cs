using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf;


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

    class LogViewModel : ReactiveObject, IEnableLogger
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



