﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Helper
{
    public static class DataTemplateHelper
    {
        public static IEnumerable<DictionaryEntry> DefaultDataTemplates(this Type type)
        {
            var dataTemplateKey = new DataTemplateKey(type);
            var dt = (DataTemplate)Application.Current.Resources[dataTemplateKey];
            yield return new DictionaryEntry("Default", dt);
        }

        public static IEnumerable<DictionaryEntry> CustomDataTemplates(this Type type, ResourceDictionary res)
        {
            foreach (var entry in res.Cast<DictionaryEntry>())
            {
                var (key, value) = entry;
                if (value is DataTemplate { DataType: Type datatype })
                {
                    if (datatype.IsAssignableFrom(type))
                    {
                        yield return entry;
                    }
                }
            }
        }
    }
}
