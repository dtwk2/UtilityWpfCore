﻿using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Common;
using Utility.Common.Enum;
using Utility.Common.EventArgs;
using Utility.ViewModel;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Data.Model;
using sim = Utility.Service.FilterDictionaryService<UtilityWpf.Demo.Data.Model.Stock>;

namespace UtilityWpf.Demo.Hybrid
{
    /// <summary>
    /// Interaction logic for GroupingUserControl.xaml
    /// </summary>
    public partial class GroupingUserControl : UserControl
    {
        public GroupingUserControl()
        {
            InitializeComponent();
        }
    }

    public class GroupingViewModel : ReactiveObject
    {
        private static string InitialPropertyName = nameof(Stock.Sector);
        private ClassProperty selected;
        private ReactiveCommand<CollectionItemEventArgs, ClassProperty?> command;

        public GroupingViewModel()
        {
            var aa = new sim(a => a.Key);
            CollectionViewModel = new(StockObservableFactory
                .GenerateChangeSet(), aa, InitialPropertyName);
            selected = CollectionViewModel.Properties.First();

            this.WhenAnyValue(a => a.Selected)
                .Select(a => (ClassProperty?)a)
                .Subscribe(CollectionViewModel);

            command = ReactiveCommand.Create<CollectionItemEventArgs, ClassProperty?>((a) =>
            {
                if (a is { EventType: EventType.Enable })
                    return Selected;
                if (a is { EventType: EventType.Disable })
                    return null;
                else
                    throw new System.Exception("dfs33 33");
            });

            command.Subscribe(CollectionViewModel);
        }

        public CollectionGroupViewModel<Stock, string> CollectionViewModel { get; }

        public ICommand Command => command;

        public ClassProperty Selected { get => selected; set => this.RaiseAndSetIfChanged(ref selected, value); }
    }
}