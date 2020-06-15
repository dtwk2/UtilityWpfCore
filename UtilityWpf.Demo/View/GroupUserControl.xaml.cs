using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

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

            var changeSet = fds();


            Combobox1.SelectAddChanges().StartWith(new[] { Combobox1.SelectedItem } as IList).Subscribe(a =>
            {

                var @switch = a.Cast<ComboBoxItem>().First().Content.ToString() switch
                {
                    "A" => sdfd(changeSet.Group(g => g.Sector)),
                    "B" => sdfd(changeSet.Group(g => g.Name.Length.ToString())),
                    _=> sdfd2(changeSet)
                };

                ListBox1.ItemsSource = @switch;
            });

            static IEnumerable sdfd(IObservable<IGroupChangeSet<Stock,string,string>> groups )
            {
                groups
          .Transform(t => new GroupViewModel<Stock>(t))
          .ObserveOnDispatcher()
          .Bind(out var data)
          //.DisposeMany()
          .Subscribe(v =>
          {
                    //this.Dispatcher.InvokeAsync(() => ListBox.ItemsSource = _data,DispatcherPriority.Background);
                });

                return data;
            }     
            
            static IEnumerable sdfd2(IObservable<IChangeSet<Stock,string>> groups )
            {
                groups
          .ObserveOnDispatcher()
          .Bind(out var data)
          //.DisposeMany()
          .Subscribe(v =>
          {
                    //this.Dispatcher.InvokeAsync(() => ListBox.ItemsSource = _data,DispatcherPriority.Background);
                });

                return data;
            }
        }

        IObservable<IChangeSet<Stock, string>> fds()
        {
            var stocks = Finance.Stocks.ToObservable().Buffer(5).Select(a => a.OrderBy(c => new Guid()).First()).Pace(TimeSpan.FromSeconds(2));

            //return stocks.Subscribe(a =>
            // {

            // });
            return stocks
                 .ToObservableChangeSet(c => c.Key);
          
        }

        public static DataTemplateSelector sdfd2 => LambdaConverters.TemplateSelector.Create<object>(e =>
        {
            return e.Item switch
            {
                Stock stock => ((FrameworkElement)e.Container)?.FindResource("StockTemplate"),
                _ => ((FrameworkElement)e.Container)?.FindResource("GroupTemplate"),
            } as DataTemplate;
        });

    }


    public class GroupViewModel<T> : ReactiveUI.ReactiveObject
    {
        private int count;

        public string Key { get; private set; }
        public int Count => count;


        public GroupViewModel(IGroup<T, string, string> group)
        {
            Key = group.Key;

            group.Cache.Connect().ToCollection()

               .Subscribe(a =>
               {
                   this.RaiseAndSetIfChanged(ref count, a.Count, nameof(Count));
               },
               e =>
               {
               });

        }


    }

}
