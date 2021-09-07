using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityWpf.Demo.Common.ViewModel;

namespace UtilityWpf.Demo.Dragablz.ViewModel
{
    class NotesViewModel: Common.ViewModel.NotesViewModel
    {

        public override ObservableCollection<NoteViewModel> Collection { get; } = new ObservableCollection<NoteViewModel> {
        
        new NoteViewModel("sdsfd"), 
        new NoteViewModel("sds333d"), 
        };

    }
}
