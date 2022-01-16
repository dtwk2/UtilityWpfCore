using System;
using System.Collections.Generic;

namespace Utility.Common.Helper
{
    public static class TypeHelper
    {
        /// <summary>
        /// <a href="https://stackoverflow.com/questions/1900353/how-to-get-the-type-contained-in-a-collection-through-reflection"></a>
        /// </summary>
        /// <param name="collectionType"></param>
        /// <returns>the inner type of a collection type e.g IEnumerable</returns>
        public static Type ElementType(this Type collectionType)
        {
            Type? ienum = FindIEnumerable(collectionType);
            if (ienum == null) return collectionType;
            return ienum.GetGenericArguments()[0];

            static Type? FindIEnumerable(Type seqType)
            {
                if (seqType == null || seqType == typeof(string))
                    return null;
                if (seqType.IsArray)
                    return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
                if (seqType.IsGenericType)
                {
                    foreach (Type arg in seqType.GetGenericArguments())
                    {
                        Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                        if (ienum.IsAssignableFrom(seqType))
                        {
                            return ienum;
                        }
                    }
                }
                Type[] ifaces = seqType.GetInterfaces();
                if (ifaces != null && ifaces.Length > 0)
                {
                    foreach (Type iface in ifaces)
                    {
                        Type? ienum = FindIEnumerable(iface);
                        if (ienum != null)
                            return ienum;
                    }
                }
                if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                {
                    return FindIEnumerable(seqType.BaseType);
                }
                return null;
            }
        }
    }
}