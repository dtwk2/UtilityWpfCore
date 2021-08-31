using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using UtilityWpf.Property;

namespace UtilityWpf.Mixins
{
    public interface INameTypeDictionary { }

    public class NameTypeDictionary<TValue> : INameTypeDictionary, IEnumerable<KeyValuePair<string, TValue>> where TValue : new()
    {
        private readonly Dictionary<string, TValue> subjects = new();
        private readonly Dictionary<Type, string> nameTypeDictionary = new();
        private readonly Lazy<DependencyProperty[]> propertyInfos;

        public NameTypeDictionary(object @object)
        {
            this.propertyInfos = new(() => @object.GetType().SelectDependencyProperties().ToArray());
        }

        public TValue this[string name]
        {
            get => GetValue(name);
        }

        public TValue this[Type type]
        {
            get => GetValue(type);
        }

        public TValue GetValue(Type type)
        {
            return GetValue(GetName(type));
        }

        private string GetName(Type type)
        {
            if (nameTypeDictionary.ContainsKey(type))
            {
                return nameTypeDictionary[type];
            }
            else
            {
                var where = propertyInfos.Value.Where(a => a.PropertyType == type).ToArray();
                if (where.Any())
                    if (where.Length == 1)
                    {
                        if (nameTypeDictionary.ContainsKey(type))
                            throw new Exception($"dictionary already contains type, {type.Name}!");
                        return nameTypeDictionary[type] = where.Single().Name;
                    }
                    else
                        throw new Exception("UnExpected multiple types");
                else
                    throw new Exception("No types match");
            }
        }

        private TValue GetValue(string? name = null)
        {
            if (name == null) throw new ArgumentException("666jj");

            if (subjects.ContainsKey(name))
                return subjects[name];

            var where = propertyInfos.Value.Where(a => a.Name == name).ToArray();
            if (where.Any())
                if (where.Length == 1)
                {
                }
                else if (where.Length > 1)
                    throw new Exception("UnExpected multiple types");

            return subjects[name] = new();
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return subjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return subjects.GetEnumerator();
        }


        //public KeyValuePair<string, TValue> Current => Enumerator.Current;

        //object IEnumerator.Current => Enumerator.Current;

        //public void Dispose()
        //{
        //    Enumerator.Dispose();
        //    enumerator = null;
        //}
        //public bool MoveNext()
        //{
        //    return Enumerator.MoveNext();
        //}

        //public void Reset()
        //{
        //    Enumerator.Reset();
        //}
    }
}