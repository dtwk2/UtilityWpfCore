using ReactiveUI;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Demo.Data.Factory;

public abstract class ProfileFilter : ReactiveObject, IPredicate, IKey
{
    public ProfileFilter(string header)
    {
        Header = header;
    }

    public string Header { get; }

    public abstract bool Invoke(object value);

    public string Key => Header;
}