using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReactiveUI;
using Utility.Common;
using UtilityWpf.Controls;
using UtilityHelperEx;
using Microsoft.Xaml.Behaviors.Core;

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

        public ICommand Command { get; init; }

        public string Header { get; init; }

    }

    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>
            {
                new("1", new ActionCommand(()=>{ })),
                new("2", ReactiveCommand.Create(()=>{ })),
                new("3", ReactiveCommand.Create(()=>{ })),
            };
        }

        public ObservableCollection<ButtonViewModel> Data { get; }

    }


    public class MethodsViewModel
    {
        public MethodsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>(
                ReflectionHelper.GetMethods(new Model())
                .Select(a => new ButtonViewModel(a.Item1, ReactiveCommand.Create(() => { _ = a.Item2(); }))));                
        }

        public ObservableCollection<ButtonViewModel> Data { get; }

    }

    class Model
    {
        [Description("One")]
        public void ShowOne()
        {
            MessageBox.Show("One");
        }
              
        [Description("Two")]
        public void ShowTwo()
        {
            MessageBox.Show("Two");
        }
               
        [Description("Three")]
        public void ShowThree()
        {
            MessageBox.Show("Three");
        }

    }

}