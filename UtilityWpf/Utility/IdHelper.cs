using System;
using System.Linq;
using UtilityHelper;

namespace UtilityWpf
{
    // Also in UtilityWpf.CommonCore in Reflection.cs
    public static class IdHelper
    {
        private static readonly string[] idnames = { "id", "key", "name", "identification" };

        public static string GetIdProperty<T>() => GetIdProperty(typeof(T));

        public static string GetIdProperty(Type type)
        {
            var pnames = type.GetProperties().Select(a => a.Name);
            return pnames.FirstOrDefault(name => idnames.Contains(name.ToLower()));
        }

        public static bool CheckIdProperty(string id, Type type)
        {
            var pt = type.GetProperty(id).PropertyType;
            var interfaces = pt.GetInterfaces();
            if (!type.IsAssignableFrom(typeof(IConvertible)) && !interfaces.Select(a => a.Name).Any(a => a.StartsWith("IEquatable")))
                return false;
            return true;
        }
    }
}