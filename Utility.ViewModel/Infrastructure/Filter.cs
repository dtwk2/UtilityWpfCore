using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility.ViewModel;

public class Filter<T> : IObserver<Dictionary<string, bool>>, IObservable<Func<T, bool>>
{
    //private readonly Func<T, string> func;
    private Dictionary<string, bool>? value;

    private Subject<Unit> subject = new();

    public Filter(Func<T, string> func)
    {
        Func = func;
    }

    public bool Excecute(T profile)
    {
        var property = Func(profile);
        if (value == null || value.ContainsKey(property) == false)
        {
            return true;
        }
        var re = value[property];
        //if (re == false)
        //{
        //}
        return re;
    }

    public Func<T, string> Func { get; }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Dictionary<string, bool> value)
    {
        this.value = value;
        subject.OnNext(default);
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return subject.Select(a => Excecute).Subscribe(observer);
    }
}