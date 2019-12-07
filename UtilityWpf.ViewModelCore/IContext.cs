using System;
using System.Threading.Tasks;

namespace UtilityWpf
{
    public interface IContext
    {
        //bool IsSynchronized { get; }
        void Invoke(Action action);

        Task InvokeAsync(Action action);
    }
}