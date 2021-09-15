using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace UtilityWpf.Demo.Common.ViewModel
{
    public class TickViewModel : ReactiveObject
    {

        private bool isChecked;

        public TickViewModel(bool isChecked, string text = "Tick")
        {
            IsChecked = isChecked;
            Text = text;
            Command = ReactiveCommand.Create(() =>
            {
                IsChecked = !IsChecked;
            });
        }

        public bool IsChecked { get => isChecked; set => this.RaiseAndSetIfChanged(ref isChecked, value); }
        public string Text { get; }

        public ICommand Command { get; } 
    }
}
