using DynamicData;
using ReactiveUI;
using System;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Demo.Data.Model;

public abstract class Filter : ReactiveObject, IPredicate, IKey
{
    protected Filter(string header)
    {
        Header = header;
    }

    public string Header { get; }

    public abstract bool Invoke(object value);

    public string Key => Header;
}

public abstract class ObserverFilter<T> : Filter, IObserver<IChangeSet<T>>
{
    protected ObserverFilter(string header) : base(header)
    {
    }

    public virtual void OnCompleted()
    {
        //throw new NotImplementedException();
    }

    public virtual void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public abstract void OnNext(IChangeSet<T> value);
}