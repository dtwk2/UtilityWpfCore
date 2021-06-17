using System;
using System.Windows.Forms;
using System.Windows.Input;
using UtilityWpf.Property;

namespace UtilityWpf.Command
{
    public class FolderOpenCommand : NPC, ICommand
    {
        private string directory;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public string Directory
        {
            get { return directory; }
            set { OnPropertyChanged(ref directory, value); }
        }

        public void Execute(object parameter)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    Directory = fbd.SelectedPath;
            }
        }
    }
}