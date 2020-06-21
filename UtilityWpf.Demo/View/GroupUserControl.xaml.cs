using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.ViewModel;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for GroupUserControl.xaml
    /// </summary>
    public partial class GroupUserControl : UserControl
    {
        public GroupUserControl()
        {
            InitializeComponent();

            var changeSet = GenerateChangeSet();


            Combobox1.SelectAddChanges().StartWith(new[] { Combobox1.SelectedItem } as IList).Subscribe(a =>
            {

                var @switch = a.Cast<ComboBoxItem>().First().Content.ToString() switch
                {
                    "A" => new GroupMasterViewModel<Stock, string, string>(changeSet, g => g.Sector).Collection,
                    "B" => new GroupMasterViewModel2(changeSet.Group( g => g.Name.Length.ToString())).Collection,
                    _ => (IEnumerable)CollectStocks(changeSet)
                };

                ListBox1.ItemsSource = @switch;
            });



            static ReadOnlyObservableCollection<Stock> CollectStocks(IObservable<IChangeSet<Stock, string>> changeSet)
            {
                changeSet
          .ObserveOnDispatcher()
          .Bind(out var data)
          //.DisposeMany()
          .Subscribe(v =>
          {
          });
                return data;
            }
        }

        IObservable<IChangeSet<Stock, string>> GenerateChangeSet()=>
            Finance.Stocks
            .ToObservable()
            .Buffer(5)
            .Select(a => a.OrderBy(c => new Guid()).First())
            .Pace(TimeSpan.FromSeconds(2))  
            .ToObservableChangeSet(c => c.Key);

        

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

        public GroupViewModel2(IGroup<Stock, string, string> group):base(group)
        {
            maxLength = group.Cache.Connect().ToCollection()
                .Select(a => a.Select(a => a.Sector.Length).Max())
                .ToProperty(this, a => a.MaxLength);
        }

        public int MaxLength => maxLength.Value;

    }
}
