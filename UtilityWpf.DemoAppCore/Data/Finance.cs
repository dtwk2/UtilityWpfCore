using Endless;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilityWpf.DemoApp
{
    public static class Finance
    {
        static ICachedEnumerable<Sector> sectors = SelectSectors().Cached();
        static IEnumerable<Sector> SelectSectors()
        {
            var reader = new StreamReader("../../../stocknet-dataset-master/StockTable.csv");

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

        public static IEnumerable<Sector> Sectors => sectors;

        public static IEnumerable<Stock> Stocks => sectors.SelectMany(a => a.Stocks);


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
        //public Series<DateTime, double> Prices { get; set; }
        //public IEnumerable<DayMovement> Prices { get; set; }
    }
}

