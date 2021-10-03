using System.Collections.Generic;
using System.Linq;
using UtilityInterface.NonGeneric.Data;

namespace Utility.Common.Helper
{
    public static class RepositoryHelper
    {

        public static IEnumerable<T> FindAll<T>(this IRepository repository)
        {
            return repository.FindMany(new AllQuery()).Cast<T>();
        }

        public static int Count(this IRepository repository)
        {
            return (int)repository.FindBy(new CountQuery());
        }

        public static T First<T>(this IRepository repository)
        {
            return (T)repository.FindBy(new FirstQuery());
        }
    }
}
