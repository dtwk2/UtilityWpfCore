﻿using System.Collections.Generic;
using System.Windows.Controls;

namespace UtilityWpf.Demo.Controls
{
    /// <summary>
    /// Interaction logic for DictionaryUserControl.xaml
    /// </summary>
    public partial class DictionaryUserControl : UserControl
    {
        public DictionaryUserControl()
        {
            InitializeComponent();
        }
    }

    public class MyDictionary : Dictionary<string, int>
    { }
}