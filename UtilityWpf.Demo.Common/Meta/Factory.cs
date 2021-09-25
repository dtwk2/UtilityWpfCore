using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityHelper;
using UtilityWpf.Demo.Common.ViewModel;
using Utility.Common.Helper;
using Splat;
using System.Collections.ObjectModel;

namespace UtilityWpf.Demo.Common.Meta
{
    public class FactoryLogger
    {
        public record FactoryLog(DateTime Date, string Key, string Type);

       // readonly Kaos.Collections.RankedSet<FactoryLog> logs = new Kaos.Collections.RankedSet<FactoryLog>(Comparer<FactoryLog>.Create(new Comparison<FactoryLog>((a, b) => (int)(a.Date - b.Date).Ticks)));
        readonly ObservableCollection<FactoryLog> logs = new ObservableCollection<FactoryLog>();

        public void Add(DateTime Date, string Key, string Type)
        {
            logs.Add(new FactoryLog(Date, Key, Type));
        }

        public IReadOnlyCollection<FactoryLog> Logs => logs;
    }


    public class Factory
    {
        FactoryLogger logger = Locator.Current.GetService<FactoryLogger>() ?? throw new Exception("mm,,ffjr");

        public T Create<T>() where T : class
        {
            var type = typeof(T);
            if (type == typeof(TickViewModel))
            {
                var t = Shelf.TickViewModel;
                logger.Add(DateTime.Now, t.Text, typeof(T).Name);
                return (t as T)!;
            }
            else if (type == typeof(ButtonViewModel))
            {
                var t = Shelf.ButtonViewModel;
                logger.Add(DateTime.Now, t.Header, typeof(T).Name);
                return (t as T)!;
            }
            else if (type == typeof(Fields))
            {
                var t = Shelf.Fields;
                logger.Add(DateTime.Now, t.Name, typeof(T).Name);
                return (t as T)!;
            }
            throw new ArgumentOutOfRangeException("s,,,,dfsdf");
        }

        public IEnumerable<T> Create<T>(int count) where T : class => EnumerableHelper.Create(count, Create<T>);


        class Shelf
        {
            static KeyStore Store = Locator.Current.GetService<KeyStore>() ?? throw new Exception("m6776m,,ffjr");

            public static TickViewModel TickViewModel => new TickViewModel(Guid.NewGuid(), Store.CreateNewKey()) { IsChecked = Statics.Random.NextBoolean() };
            public static ButtonViewModel ButtonViewModel => new ButtonViewModel(Store.CreateNewKey(), ReactiveCommand.Create(() => { })) { IsRefreshable = Statics.Random.NextBoolean() };
            public static Fields Fields => new Fields
            {
                Age = Statics.Random.Next(0, 100),
                Id = Guid.NewGuid(),
                Name = RandomHelper.NextWord(rand: Statics.Random),
                Surname = RandomHelper.NextWord(rand: Statics.Random)
            };
        }
    }
}
