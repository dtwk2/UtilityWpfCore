using System;
using System.Threading.Tasks;

namespace UtilityWpf
{
    public interface IDispatcher
    {
        void Invoke(Action action);

        Task InvokeAsync(Action action);

        bool CheckAccess();
    }
}
