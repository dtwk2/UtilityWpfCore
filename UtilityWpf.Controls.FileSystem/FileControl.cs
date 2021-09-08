using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls.FileSystem
{
    public class FileControl : PathControl
    {


        static FileControl()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(FileControl), new FrameworkPropertyMetadata(typeof(FileControl)));
        }

        public FileControl()
        {
            Icon = PackIconKind.File;
        }

    }
}
