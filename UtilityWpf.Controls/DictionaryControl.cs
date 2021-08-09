﻿using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    // for displaying key value pairs
    public class DictionaryControl : ItemsControl
    {
        static DictionaryControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DictionaryControl), new FrameworkPropertyMetadata(typeof(DictionaryControl)));
        }

        public DictionaryControl()
        {
        }
    }
}