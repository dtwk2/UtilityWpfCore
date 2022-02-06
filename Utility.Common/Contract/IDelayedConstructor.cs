using System.Threading.Tasks;

namespace Utility.Common.Contract
{
    public interface IDelayedConstructor
    {
        Task<bool> Init(object o);
    }
}