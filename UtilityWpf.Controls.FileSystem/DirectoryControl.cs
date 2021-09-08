using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace UtilityWpf.Controls.FileSystem
{
    public class DirectoryControl : PathControl
    {

        static DirectoryControl()
        {
          //  DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryControl), new FrameworkPropertyMetadata(typeof(DirectoryControl)));
        }

        public DirectoryControl()
        {
            Icon = PackIconKind.Folder;
        }
    }
}
