using System;

namespace UtilityWpf
{
    public class AutoResourceDictionary : SharedResourceDictionary
    {
        private Type? type;

        public Type Type
        {
            set
            {
                if (type == value)
                    return;
                type = value;
                foreach (var resourceDictionary in type.Assembly.SelectResourceDictionaries())
                {
                    AddToMergedDictionaries(resourceDictionary);
                }
            }
        }
    }
}