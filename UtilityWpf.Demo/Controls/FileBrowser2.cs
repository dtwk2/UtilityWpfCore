using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Controls.Browser;

namespace Utility.WPF.Demo.Controls
{
    public class FileBrowser2 : FileBrowser
    {
        private TextBlock TextBlockOne;

        public FileBrowser2()
        {
            TextBlockOne = new TextBlock { Width = 300, VerticalAlignment = VerticalAlignment.Center };
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
