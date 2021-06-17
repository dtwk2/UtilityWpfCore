using System.Threading.Tasks;
using UtilityWpf.Model;

namespace UtilityWpf.Abstract
{
    public interface IViewModelAssemblyModel
    {
        Task<TypeObject[]> Collection { get; }
    }
}