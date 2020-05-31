using System;
using System.Linq;

namespace UtilityHelper
{
    // Also in UtilityWpf.CommonCore in Reflection.cs
    public static class IdHelper
    {
        private static readonly string[] idnames = { "id", "key", "name", "identification" };

        public static string GetIdProperty<T>() => GetIdProperty(typeof(T));

        public static string GetIdProperty(Type type)
        {
            var pnames = type.GetProperties().Select(_ => _.Name);
            return (pnames.FirstOrDefault(_ => idnames.Contains(_.ToLower())));
        }

        public static bool CheckIdProperty(string id, Type type)
        {
            var pt = type.GetProperty(id).PropertyType;
            var interfaces = pt.GetInterfaces();
            if (!type.IsAssignableFrom(typeof(IConvertible)) && !interfaces.Select(_ => _.Name).Any(_ => _.StartsWith("IEquatable")))
                return false;
            return true;
        }
    }
}