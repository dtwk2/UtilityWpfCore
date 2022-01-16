using System;
using System.Text.Json;

namespace Utility.Common.Helper
{
    public static class TextJsonHelper
    {
        public static object? Deserialise(string str, Type type)
        {

            var options = new JsonSerializerOptions();
            return System.Text.Json.JsonSerializer.Deserialize(str, type, options);
        }
    }
}