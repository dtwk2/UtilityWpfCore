using Endless;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UtilityWpf.Demo.Common.ViewModel;

namespace UtilityWpf.Demo.Dragablz
{
    /// <summary>
    /// Interaction logic for TicksUserControl.xaml
    /// </summary>
    public partial class ResourceDictionariesUserControl : UserControl
    {
        public ResourceDictionariesUserControl()
        {
            InitializeComponent();
        }
    }



    public class ResourceDictionariesViewModel
    {
        private ICommand changeCommand;
        private bool isReadOnly;
        Subject<TickChange> subject = new();
        Dictionary<ResourceDictionary, bool> dictionary = new();
        public ResourceDictionariesViewModel()
        {
            subject.Subscribe(ad =>
            {
                var dict = dictionary.Keys.Single(a => a.Source.ToString() == ad.Text);
                dictionary[dict] = ad.NewValue;
                if (ad.NewValue)
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                else
                    Application.Current.Resources.MergedDictionaries.Remove(dict);
            });

            Collection = new();
            foreach (var dic in Application.Current.Resources.MergedDictionaries.Where(a => a.Source != null))
            {
                var tick = new TickViewModel(true, dic.Source.ToString());
                dictionary[dic] = true;
                tick.Subscribe(subject);
                Collection.Add(tick);
            }
        }

        public bool IsReadOnly
        {
            get => isReadOnly; set => isReadOnly = value;
        }
        public string Header { get; } = "Resource-Dictionaries ViewModel";

        public ObservableCollection<TickViewModel> Collection { get; }


        public ICommand ChangeCommand => changeCommand ??= new ActionCommand(Change);

        private void Change()
        {

        }
    }

 
}
