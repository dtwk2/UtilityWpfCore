using System;
using System.Threading.Tasks;

namespace Utility.Common.Contract
{
    public interface IDispatcher
    {
        void Invoke(Action action);

        Task InvokeAsync(Action action);

        bool CheckAccess();
    }
}