using System.Threading.Tasks;

namespace UtilityWpf
{
    public interface IDelayedConstructor
    {
        Task<bool> Init(object o);
    }
}