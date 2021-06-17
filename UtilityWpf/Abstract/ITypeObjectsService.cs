using System;
using UtilityWpf.Model;

namespace UtilityWpf.Abstract
{
    public interface ITypeObjectsService
    {
        TypeObject[] SelectTypeObjects(Type[] types);
    }
}
