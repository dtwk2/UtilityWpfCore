using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Endless;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Input;
using UtilityWpf.TestData.Model;
using System.Collections.ObjectModel;
using NetFabric.Hyperlinq;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class TicksViewModel
    {
        private ICommand changeCommand;
        static Random rand = new();
        private bool isReadOnly;
        public bool IsReadOnly
        {
            get => isReadOnly; set => isReadOnly = value;
        }
        public string Header { get; } = "NotesViewModel";

        public ObservableCollection<TickViewModel> Collection { get; } = new ObservableCollection<TickViewModel> { new TickViewModel(false), new TickViewModel(true) };

        public System.Collections.IEnumerator NewItem { get => 0.Repeat().Select(a => new TickViewModel(rand.Next(0, 2) == 0)).GetEnumerator(); }

        public ICommand ChangeCommand => changeCommand ??= new ActionCommand(Change);

        private void Change()
        {
        }
    }
}
