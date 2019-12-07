using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for TreeViewUserControl.xaml
    /// </summary>
    public partial class TreeViewUserControl : UserControl
    {
        private Stock stock;
        private ReadOnlyObservableCollection<Sector> sectors;

        public ReadOnlyObservableCollection<Sector> Sectors => sectors;

        public Stock Stock
        {
            get => stock;
            set => stock = (stock != value) ? value : stock;
        }

        public TreeViewUserControl()
        {
            InitializeComponent();
            Grid.DataContext = this;
            var dis1 =
        GetSectors()
         .ToObservable(Scheduler.Default)
         .ToObservableChangeSet()
         .ObserveOnDispatcher()
            .DisposeMany()
         .Bind(out sectors)
         .Subscribe(_ =>
         Stock = sectors.First().Stocks.First());
        }

        public IEnumerable<Sector> GetSectors()
        {
            var reader = new StreamReader("../../../stocknet-dataset-master/StockTable.csv");

            return from myRow in Csv.CsvReader.Read(reader)
                   group myRow by myRow["Sector"] into g
                   select new Sector
                   {
                       Key = g.Key,
                       Stocks = g.Select(__ => new Stock
                       {
                           Key = __["Symbol"].Remove(0, 1),
                           Name = __["Company"]
                       }).ToList()
                   };
        }
    }

    public class Sector
    {
        public string Key { get; set; }
        public ICollection<Stock> Stocks { get; set; }
    }

    public class Stock
    {
        public string Name { get; set; }
        public string Key { get; set; }
        //public Series<DateTime, double> Prices { get; set; }
        //public IEnumerable<DayMovement> Prices { get; set; }
    }
}