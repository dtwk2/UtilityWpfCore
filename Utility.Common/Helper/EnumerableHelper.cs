using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common.Helper
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> Create<T>(int count, Func<T> create)
        {
            for (int i = 0; i < count; i++)
            {
                yield return create();
            }
        }
    }
}
