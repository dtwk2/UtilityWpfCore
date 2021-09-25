using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Utility.Common
{
    public class DefaultComparer<T, R> : IComparer<T> where R : IComparable<R>
    {
        private readonly Func<T, R> func;

        public DefaultComparer(Func<T, R> func)
        {
            this.func = func;
        }

        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            return func(x).CompareTo(func(y));
        }
    }
}