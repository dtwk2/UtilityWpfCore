using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UtilityWpf.Controls.FileSystem;

namespace UtilityWpf.Demo.FileSystem.Infrastructure
{
    public class FileBrowser2 : FileBrowser
    {
        private TextBlock TextBlockOne;

        public FileBrowser2()
        {
            TextBlockOne = new TextBlock { Width = 300, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            this.TextBoxContent = TextBlockOne;
        }

        protected override void OnTextChange(string path, TextBox textBox)
        {
            TextBlockOne.Text = path;
            TextBlockOne.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            TextBlockOne.ToolTip = path;
        }
    }
}
