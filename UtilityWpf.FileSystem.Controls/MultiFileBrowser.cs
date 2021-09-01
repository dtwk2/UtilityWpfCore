using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Controls.FileSystem
{
    public class MultiFileBrowser : FileBrowser<ListBox>
    {

        public MultiFileBrowser()
        {
            this.TextBoxContent = new ListBox();
            fileBrowserCommand.IsMultiSelect = true;
        }


        protected override void OnTextChange(string path, ListBox sender)
        {
            ((ObservableCollection<string>)(sender.ItemsSource ??= new ObservableCollection<string>())).Add(path);
            base.OnTextChange(path, sender);
        }
    }
}
