﻿using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using UtilityHelperEx;
using UtilityWpf.Demo.Common.Meta;

namespace UtilityWpf.Demo.Common.ViewModel
{
    public class ViewModel
    {

    }
    public class ButtonViewModel: ViewModel
    {

        public ButtonViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public ICommand Command { get; set; }

        public bool IsRefreshable { get; init; }

        public string Header { get; init; }
    }

    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = (Locator.Current.GetService<Factory>() ?? throw new Exception("df___fsd")).Create<ButtonViewModel>(3).ToObservableCollection();
        }

        public ObservableCollection<ButtonViewModel> Data { get; } 

    }
}
