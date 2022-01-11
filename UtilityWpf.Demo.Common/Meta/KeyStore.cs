using System.Collections.Generic;
using UtilityHelper;

namespace UtilityWpf.Demo.Common.Meta
{
    internal class KeyStore
    {
        public static HashSet<string> Keys { get; } = new();

        public string CreateNewKey(string word = "")
        {
            while (string.IsNullOrEmpty(word) || Keys.Add(word) == false)
                word = RandomHelper.NextWord(5, Statics.Random);
            return word;
        }
    }
}