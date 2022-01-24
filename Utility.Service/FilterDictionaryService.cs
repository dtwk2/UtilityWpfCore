using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.NonGeneric;

namespace Utility.Service;

public interface IFilterService<T> : IObservable<Func<T, bool>>
{
}

public class FilterService<T> : IObserver<Func<T, bool>>, IFilterService<T>
{
    private readonly Subject<Unit> subject = new();
    private Func<T, bool>? value;

    public bool Execute(T profile)
    {
        if (value == null || value.Invoke(profile))
        {
            return true;
        }
        return false;
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Func<T, bool> value)
    {
        this.value = value;
        subject.OnNext(default);
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return Observable.Select<Unit, Func<T, bool>>(subject, a => Execute).Subscribe(observer);
    }
}

public class FilterDictionaryService<T> : IObserver<Dictionary<string, bool>>, IFilterService<T>
{
    private readonly FilterService<T> filterBaseService;

    public FilterDictionaryService(Func<T, string> keyFunc)
    {
        KeySelector = keyFunc;
        filterBaseService = new FilterService<T>();
    }

    public Func<T, string> KeySelector { get; }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Dictionary<string, bool> dictionary)
    {
        filterBaseService.OnNext(new Func<T, bool>(value =>
        {
            if (dictionary.ContainsKey(KeySelector(value)) == false)
            {
                return true;
            }
            return dictionary[KeySelector(value)];
        }));
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return filterBaseService.Subscribe(observer);
    }
}

public class FilterPredicateService<T> : IObserver<IPredicate>, IFilterService<T>
{
    private readonly FilterService<T> filterBaseService;

    public FilterPredicateService()
    {
        filterBaseService = new FilterService<T>();
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(IPredicate predicate)
    {
        filterBaseService.OnNext(new Func<T, bool>(value =>
        {
            if (predicate.Invoke(value) == false)
            {
                return true;
            }
            return false;
        }));
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return filterBaseService.Subscribe(observer);
    }
}
