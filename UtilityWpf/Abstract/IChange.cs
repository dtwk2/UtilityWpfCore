using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UtilityWpf.Abstract
{

    public enum EventType
    {
        Add, Remove, Removed, MoveUp, MoveDown
    }


    public class CollectionChangedEventArgs : CollectionEventArgs
    {
        public CollectionChangedEventArgs(IEnumerable array, IReadOnlyCollection<object> changes, EventType eventType, object? item, int index, RoutedEvent @event) : base(eventType, item, index, @event)
        {
            Objects = array;
            Changes = changes;
        }

        public IEnumerable Objects { get; }
        public IReadOnlyCollection<object> Changes { get; }
    }

    public class CollectionEventArgs : RoutedEventArgs
    {
        public CollectionEventArgs(EventType eventType, object? item, int index, RoutedEvent @event) : base(@event)
        {
            EventType = eventType;
            Item = item;
            Index = index;
        }

        public EventType EventType { get; }

        public object? Item { get; }
        public int Index { get; }
    }

    public delegate void CollectionChangedEventHandler(object sender, CollectionEventArgs e);

    public interface IChange
    {
        event CollectionChangedEventHandler Change;
    }
}
