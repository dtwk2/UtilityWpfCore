using System;
using System.Collections;
using System.Collections.Generic;
using Utility.Common.Enum;

namespace Utility.Common.EventArgs
{
    public class CollectionChangedEventArgs : CollectionEventArgs
    {
        public CollectionChangedEventArgs(IEnumerable array, IReadOnlyCollection<object> changes, EventType eventType, object item, int index) : base(eventType, item, index)
        {
            Objects = array;
            Changes = changes;
        }

        public IEnumerable Objects { get; }
        public IReadOnlyCollection<object> Changes { get; }
    }

    public class CollectionEventArgs
    {
        public CollectionEventArgs(EventType eventType, object item, int index)
        {
            EventType = eventType;
            Item = item;
            Index = index;
        }

        public EventType EventType { get; }

        public object Item { get; }
        public int Index { get; }
    }

    public class MovementEventArgs : CollectionEventArgs
    {
        public MovementEventArgs(IReadOnlyCollection<IndexedObject> array, IReadOnlyCollection<IndexedObject> changes, EventType eventType, object item, int index) : base(eventType, item, index)
        {
            Objects = array;
            Changes = changes;
        }

        public IReadOnlyCollection<IndexedObject> Objects { get; }
        public IReadOnlyCollection<IndexedObject> Changes { get; }
    }

    public class IndexedObject
    {
        public IndexedObject(object @object, int index, int oldIndex)
        {
            Object = @object;
            Index = index;
            OldIndex = oldIndex;
        }
        public int Index { get; set; }
        public int OldIndex { get; }
        public object Object { get; }
    }

}
