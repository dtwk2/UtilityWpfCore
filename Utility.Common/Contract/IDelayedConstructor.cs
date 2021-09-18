using System.Threading.Tasks;

namespace UtilityWpf.Abstract
{
    public interface IDelayedConstructor
    {
        Task<bool> Init(object o);
    }
}