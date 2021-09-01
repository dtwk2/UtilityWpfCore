using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace UtilityWpf.TestData.Model
{
    public class TickViewModel : ReactiveObject
    {

        private bool text;

        public TickViewModel(bool isChecked)
        {
            IsChecked = isChecked;
        }

        public bool IsChecked { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }

    }
}
