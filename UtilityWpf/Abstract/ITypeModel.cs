using System;
using System.Threading.Tasks;

namespace UtilityWpf.Abstract
{
    public interface ITypeModel
    {
        Task<Type[]> Collection { get; }
    }
}