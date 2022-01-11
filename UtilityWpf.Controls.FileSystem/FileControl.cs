﻿using MaterialDesignThemes.Wpf;

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