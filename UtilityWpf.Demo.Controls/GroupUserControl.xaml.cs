﻿using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Utility.Common;
using Utility.ViewModel;
using UtilityHelper.NonGeneric;
using UtilityHelperEx;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Controls
{
    /// <summary>
    /// Interaction logic for GroupUserControl.xaml
    /// </summary>
    public partial class GroupUserControl : UserControl
    {
        private readonly Subject<ClassProperty> subject = new Subject<ClassProperty>();

        public GroupUserControl()
        {
            InitializeComponent();

            var groupUserControlViewModel = new GroupUserControlViewModel();

            Combobox1.ItemsSource = groupUserControlViewModel.Types;

            _ = Combobox1
                .SelectSelectionAddChanges()
                .Select(a => a.First().ToString())
                .Subscribe(a =>
                {
                    groupUserControlViewModel.OnNext(a);
                });

            _ = ComboBox
                 .SelectSelectionAddChanges()
                 .Select(a => a.Cast<ClassProperty>().First())
                 .Subscribe(a =>
                 {
                     subject.OnNext(a);
                 });

            groupUserControlViewModel.WhenAnyValue(a => a.Collection)
                .BindTo(this, a => a.ListBox1.ItemsSource);

            var group2UserControlViewModel = new Group2UserControlViewModel(subject);

            group2UserControlViewModel.WhenAnyValue(a => a.ErrorMessage)
                .ObserveOnDispatcher()
                .Subscribe(a => CompletedLabel.Content = a);

            ListBox2.ItemsSource = group2UserControlViewModel.Collection;

            var group3UserControlViewModel = StockObservableFactory.GenerateUnlimitedGroupableChangeSet(subject).ToGroupOnViewModel();

            ListBox3.ItemsSource = group3UserControlViewModel.Collection;
            ComboBox.ItemsSource = group3UserControlViewModel.Properties;
        }

        public class Group2UserControlViewModel : ReactiveObject
        {
            private IObservable<IChangeSet<Groupable<Stock>, string>> changeSet;
            private GroupCollectionViewModel viewmodel;

            public Group2UserControlViewModel(IObservable<ClassProperty> subject)
            {
                changeSet = StockObservableFactory.GenerateLimitedGroupableChangeSet(subject).RefCount();

                _ = changeSet
                    .Subscribe(a => { }, () =>
                    {
                        ErrorMessage = "Observer OnCompleted method called - unable to modify grouping list - hereafter";
                        this.RaisePropertyChanged(nameof(ErrorMessage));
                    });
                viewmodel = changeSet.ToGroupOnViewModel();
            }

            public string ErrorMessage { get; private set; }
            public ICollection Collection => viewmodel.Collection;
            public IEnumerable Properties => viewmodel.Properties;
        }

        public class GroupUserControlViewModel : ReactiveObject, IObserver<string>
        {
            private readonly Subject<string> subject = new();
            private readonly ReplaySubject<IChangeSet<Stock, string>> changeSet = new();
            private readonly ObservableAsPropertyHelper<ICollection> collection;

            public GroupUserControlViewModel()
            {
                StockObservableFactory
                    .GenerateChangeSet()
                    .Subscribe(a =>
                    {
                        changeSet.OnNext(a);
                    });
                collection = subject.Select(ChangeCollection).ToProperty(this, a => a.Collection);
            }

            private ICollection ChangeCollection(string str)
            {
                return str switch
                {
                    "Sector" => changeSet.ToGroupViewModel(g => g.Sector).Collection,
                    "Arbitrary" => new CustomGroupCollectionViewModel(changeSet.Group(g => g.Name.Length.ToString())).Collection,
                    _ => CollectStocks(changeSet)
                };

                static ReadOnlyObservableCollection<Stock> CollectStocks(IObservable<IChangeSet<Stock, string>> changeSet)
                {
                    changeSet
                        .ObserveOnDispatcher()
                        .Bind(out var data)
                        .Subscribe();

                    return data;
                }
            }

            public ICollection Types => new[] { "Sector", "Arbitrary", "None" };

            public ICollection Collection => collection.Value;

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(string value)
            {
                subject.OnNext(value);
            }
        }

        public static DataTemplateSelector DataTemplateSelector => LambdaConverters.TemplateSelector.Create<object>(e =>
                                {
                                    return e.Item switch
                                    {
                                        Stock _ => ((FrameworkElement)e.Container)?.FindResource(new DataTemplateKey(typeof(Stock))),
                                        CustomGroupViewModel _ => ((FrameworkElement)e.Container)?.FindResource("Group2Template"),
                                        GroupViewModel<Stock, string, string> _ => ((FrameworkElement)e.Container)?.FindResource("GroupTemplate"),
                                        _ => throw new NotImplementedException(),
                                    } as DataTemplate;
                                });

        public class CustomGroupCollectionViewModel : GroupCollectionViewModel<Stock, string, string>
        {
            public CustomGroupCollectionViewModel(IObservable<IGroupChangeSet<Stock, string, string>> groups) : base(groups)
            {
            }

            public override CustomGroupViewModel Create(IGroup<Stock, string, string> group)
            {
                return new CustomGroupViewModel(group);
            }
        }

        public class CustomGroupViewModel : GroupViewModel<Stock, string, string>
        {
            private readonly ObservableAsPropertyHelper<int> maxLength;

            public CustomGroupViewModel(IGroup<Stock, string, string> group) : base(group)
            {
                maxLength = group.Cache
                    .Connect()
                    .ToCollection()
                    .Select(a => a.Select(a => a.Sector.Length).Max())
                    .ToProperty(this, a => a.MaxLength);
            }

            public int MaxLength => maxLength.Value;
        }
    }
}