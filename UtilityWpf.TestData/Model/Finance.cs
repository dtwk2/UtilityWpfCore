﻿using Endless;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace UtilityWpf.TestData
{
    public static class Finance
    {
        private static ICachedEnumerable<Sector> sectors = SelectSectors().Cached();

        private static IEnumerable<Sector> SelectSectors()
        {
            var reader = new StreamReader("../../../../UtilityWpf.TestData/stocknet-dataset-master/StockTable.csv");

            return from myRow in Csv.CsvReader.Read(reader)
                   group myRow by myRow["Sector"] into g
                   select new Sector
                   {
                       Key = g.Key,
                       Stocks = g.Select(line => new Stock
                       {
                           Key = line["Symbol"].Remove(0, 1),
                           Name = line["Company"],
                           Sector = g.Key,
                       }).ToList()
                   };
        }

        private static IEnumerable<Price> SelectPrices()
        {
            var reader = new StreamReader("../../../../UtilityWpf.TestData/stocknet-dataset-master/price/HL/ABB.csv");
            var start = DateTime.Today.AddYears(-5);
            return from line in Csv.CsvReader.Read(reader)
                   select new Price
                   {
                       Open = double.Parse(line["Open"]),
                       Close = double.Parse(line["Close"]),
                       DateTime = start.AddHours(line.Index),
                   };
        }

        public static IEnumerable<Sector> Sectors => sectors.Cached();

        public static IEnumerable<Stock> Stocks => Sectors.SelectMany(a => a.Stocks).Cached();

        public static IEnumerable<Price> Prices => SelectPrices().Cached();
    }

    public class Sector
    {
        public string Key { get; set; }
        public ICollection<Stock> Stocks { get; set; }
    }

    public class Stock
    {
        public string Sector { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class StockPropertyChanged : ReactiveObject
    {
        private bool flag;

        public StockPropertyChanged()
        {
            ChangeGroupProperty = ReactiveCommand.Create(() => { flag = !flag; this.RaisePropertyChanged(nameof(GroupProperty)); });
        }

        public StockPropertyChanged(IObservable<bool> observable)
        {
            var a = observable;
            a.Subscribe(a => { flag = !flag; this.RaisePropertyChanged(nameof(GroupProperty)); });
        }

        public string Sector { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }

        public ICommand ChangeGroupProperty { get; }

        public string GroupProperty => flag ? Sector : Name;
    }

    public class Price
    {
        public double Open { get; set; }
        public double Close { get; set; }
        public DateTime DateTime { get; set; }
    }
}