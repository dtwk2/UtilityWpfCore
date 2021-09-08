using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReactiveUI;
using UtilityWpf.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for PathButtonUserControl.xaml
    /// </summary>
    public partial class ButtonUserControl : UserControl
    {
        private readonly TypeConverter converter;

        public ButtonUserControl()
        {
            InitializeComponent();

            this.PathTextBox.Text = GeometryButton.InitialData;
            converter = TypeDescriptor.GetConverter(typeof(Geometry));
            this.PathTextBox.TextChanged += PathTextBox_TextChanged;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ErrorTextBlock.Text = string.Empty;
            try
            {
                PathButton.Data = (Geometry)converter.ConvertFrom(this.PathTextBox.Text);
            }
            catch (Exception ex)
            {
                this.ErrorTextBlock.Text = ex.Message;
            }
        }
    }

    public class ToggleViewModel:ReactiveObject
    {
       private bool isChecked =true;

       public bool IsChecked
       {
          get => isChecked;
          set => this.RaiseAndSetIfChanged(ref isChecked, value);
       }
    }


    public class ButtonViewModel
    {

        public ButtonViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public ICommand Command { get; set; }

        public string Header { get; set; }

    }

    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>
            {
                new("1", ReactiveCommand.Create(()=>{ })),
                new("2", ReactiveCommand.Create(()=>{ })),
                new("3", ReactiveCommand.Create(()=>{ })),
            };
        }

        public ObservableCollection<ButtonViewModel> Data { get; }

    }
}