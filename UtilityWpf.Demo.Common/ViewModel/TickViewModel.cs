using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace UtilityWpf.Demo.Common.ViewModel
{
    public class TickViewModel : ReactiveObject
    {

        private bool isChecked;

        public TickViewModel(bool isChecked)
        {
            IsChecked = isChecked;
        }

        public bool IsChecked { get => isChecked; set => this.RaiseAndSetIfChanged(ref isChecked, value); }

    }
}
