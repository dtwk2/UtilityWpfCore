using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityHelperEx;
using UtilityWpf.Model;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Controls
{
    /// <summary>
    /// Interaction logic for GroupUserControl.xaml
    /// </summary>
    public partial class GroupUserControl : UserControl
    {
        private readonly Subject<bool> subject = new Subject<bool>();

        public GroupUserControl()
        {
            InitializeComponent();

            var changeSet = GenerateChangeSet();

            Combobox1.SelectSelectionAddChanges().StartWith(new[] { Combobox1.SelectedItem } as IList).Subscribe(a =>
            {
                var @switch = a.Cast<ComboBoxItem>().First().Content.ToString() switch
                {
                    "A" => new GroupMasterViewModel<Stock, string, string>(changeSet, g => g.Sector).Collection,
                    "B" => new GroupMasterViewModel2(changeSet.Group(g => g.Name.Length.ToString())).Collection,
                    _ => (IEnumerable)CollectStocks(changeSet)
                };

                ListBox1.ItemsSource = @switch;
            });

            //var stocks = Finance.Stocks.Select(a => new StockPropertyChanged { Sector = a.Sector, Name = a.Name, Key = a.Key });

            var sad = GenerateChangeSet2().RefCount();

            _ = sad.Subscribe(a => { }, () =>
              {
                  this.Dispatcher.Invoke(() => CompletedLabel.Content = "Observer OnCompleted method called - unable to modify grouping list hence");
              });

            ListBox2.ItemsSource = new GroupMasterPropertyChangedViewModel<StockPropertyChanged, string, string>(sad, g => g.GroupProperty).Collection;

            ListBox3.ItemsSource = new GroupMasterPropertyChangedViewModel<StockPropertyChanged, string, string>(GenerateChangeSet4(), g => g.GroupProperty).Collection;

            Emoticon_ToggleButton.Checked += (a, b) => subject.OnNext(Emoticon_ToggleButton.IsChecked.Value);
            Emoticon_ToggleButton.Unchecked += (a, b) => subject.OnNext(Emoticon_ToggleButton.IsChecked.Value);

            static ReadOnlyObservableCollection<Stock> CollectStocks(IObservable<IChangeSet<Stock, string>> changeSet)
            {
                changeSet
                    .ObserveOnDispatcher()
                    .Bind(out var data)
                    .Subscribe(v => { });

                return data;
            }
        }

        private IObservable<IChangeSet<Stock, string>> GenerateChangeSet()
        {
            return Finance.Stocks
                .ToObservable()
                 .Buffer(5)
            .Select(a => a.OrderBy(c => new Guid()).First())
            .Pace(TimeSpan.FromSeconds(2))
            .ToObservableChangeSet(c => c.Key);
        }

        private IObservable<IChangeSet<Stock, string>> GenerateChangeSet1()
        {
            var innerSubject = new Subject<Stock>();
            var stocks = Finance.Stocks.ToObservable().Subscribe(innerSubject.OnNext);

            return innerSubject
                 .Buffer(5)
            .Select(a => a.OrderBy(c => new Guid()).First())
            .Pace(TimeSpan.FromSeconds(2))
            .ToObservableChangeSet(c => c.Key);
        }

        private IObservable<IChangeSet<StockPropertyChanged, string>> GenerateChangeSet2() =>
            Finance.Stocks
            .Select(a => new StockPropertyChanged(subject) { Sector = a.Sector, Name = a.Name, Key = a.Key })
            .ToObservable()
            //.Repeat(1000)
            .Buffer(5)
            .Select(a => a.OrderBy(c => new Guid()).First())
            .Pace(TimeSpan.FromSeconds(2))
            .ToObservableChangeSet(c => c.Key);

        private IObservable<IChangeSet<StockPropertyChanged, string>> GenerateChangeSet4()
        {
            var innerSubject = new ReplaySubject<Stock>();

            foreach (var x in Finance.Stocks)
                innerSubject.OnNext(x);

            return innerSubject
     .Select(a => new StockPropertyChanged(subject) { Sector = a.Sector, Name = a.Name, Key = a.Key })
     .Buffer(5)
 .Select(a => a.OrderBy(c => new Guid()).First())
 .Pace(TimeSpan.FromSeconds(2))
 .ToObservableChangeSet(c => c.Key);
        }

        public static DataTemplateSelector DataTemplateSelector1 => LambdaConverters.TemplateSelector.Create<object>(e =>
        {
            return e.Item switch
            {
                Stock _ => ((FrameworkElement)e.Container)?.FindResource("StockTemplate"),
                GroupViewModel2 _ => ((FrameworkElement)e.Container)?.FindResource("Group2Template"),
                GroupViewModel<Stock, string, string> _ => ((FrameworkElement)e.Container)?.FindResource("GroupTemplate"),
                _ => throw new NotImplementedException(),
            } as DataTemplate;
        });
    }

    public class GroupMasterViewModel2 : GroupMasterViewModel<Stock, string, string>
    {
        public GroupMasterViewModel2(IObservable<IGroupChangeSet<Stock, string, string>> groups) : base(groups)
        {
        }

        public override GroupViewModel<Stock, string, string> CreateViewModel(IGroup<Stock, string, string> group)
        {
            return new GroupViewModel2(group);
        }
    }

    public class GroupViewModel2 : GroupViewModel<Stock, string, string>
    {
        private readonly ObservableAsPropertyHelper<int> maxLength;

        public GroupViewModel2(IGroup<Stock, string, string> group) : base(group)
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