using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace UtilityWpf.Property
{
    public static class ReflectionHelper
    {
        public static IEnumerable RecursivePropValues(object e, string path)
        {
            List<IEnumerable> lst = new List<IEnumerable>();
            lst.Add(new[] { e });
            try
            {
                var xx = UtilityHelper.PropertyHelper.GetPropertyValue<IEnumerable>(e, path);
                foreach (var x in xx)
                    lst.Add(RecursivePropValues(x, path));
            }
            catch (Exception ex)
            {
                //
            }
            return lst.SelectMany(_ => _.Cast<object>());
        }

        public static IEnumerable<DependencyProperty> SelectDependencyProperties(this Type type) =>
             type
            .GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(DependencyProperty))
            .Select(f => f.GetValue(null))
            .Cast<DependencyProperty>();

        //Filters only highest level DependencyProperties. (i.e not those associated with inherited classes)
        public static IEnumerable<DependencyProperty> SelectDependencyPropertiesDeclaredOnly(this Type type) =>
     type
    .GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.DeclaredOnly)
    .Where(f => f.FieldType == typeof(DependencyProperty))
    .Select(f => f.GetValue(null))
    .Cast<DependencyProperty>();

        //https://stackoverflow.com/questions/4794071/how-to-enumerate-all-dependency-properties-of-control
        public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(this DependencyObject obj)
        {
            if (obj != null)
            {
                LocalValueEnumerator lve = obj.GetLocalValueEnumerator();
                while (lve.MoveNext())
                {
                    yield return lve.Current.Property;
                }
            }
        }

        //https://stackoverflow.com/questions/4794071/how-to-enumerate-all-dependency-properties-of-control
        public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.DependencyProperty != null)
                            yield return mp.DependencyProperty;
                    }
                }
            }
        }

        //https://stackoverflow.com/questions/4794071/how-to-enumerate-all-dependency-properties-of-control
        public static IEnumerable<DependencyProperty> EnumerateAttachedProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.IsAttached)
                            yield return mp.DependencyProperty;
                    }
                }
            }
        }
    }
}

//        public static T GetPropValue<T>(this Object obj, String name, Type type = null) => GetPropValue<T>(obj, (type ?? obj.GetType()).GetProperty(name));

//        public static T GetPropValue<T, R>(R obj, String name) => GetPropValue<T>(obj, typeof(R).GetProperty(name));

//        public static T GetPropValue<T>(this Object obj, PropertyInfo info = null)
//        {
//            if (info == null) return default(T);
//            object retval = info.GetValue(obj, null);
//            return retval == null ? default(T) : (T)retval;
//        }

//        public static IEnumerable<T> GetPropertyValues<T>(this IEnumerable obj, String name, Type type = null)
//        {
//            var info = (type ?? obj.First().GetType()).GetProperty(name);
//            foreach (var x in obj)
//                yield return GetPropValue<T>(x, info);
//        }

//        public static IEnumerable<T> GetPropertyValues<T>(this IEnumerable<Object> obj, String name, Type type = null)
//        {
//            var x = (type ?? obj.First().GetType()).GetProperty(name);
//            return obj.Select(_ => GetPropValue<T>(_, x));
//        }

//        public static IEnumerable<T> GetPropertyValues<T, R>(IEnumerable<R> obj, String name)
//        {
//            var x = typeof(R).GetProperty(name);
//            return obj.Select(_ => GetPropValue<T>(_, x));
//        }

//        public static IEnumerable<T> GetPropertyValues<T>(this IEnumerable<Object> obj, PropertyInfo info = null) => obj.Select(_ => GetPropValue<T>(_, info));

//        public static IEnumerable<T> GetPropertyValues<T>(this IEnumerable obj, PropertyInfo info = null)
//        {
//            foreach (var x in obj)
//                yield return GetPropValue<T>(x, info);
//        }

//        public static Dictionary<string, object> FromObject(object @object)
//        {
//            var props = @object.GetType().GetProperties();

//            return props.Select(_ => new { _.Name, Value = @object.GetPropValue(_) }).ToDictionary(_ => _.Name, _ => _.Value);

//        }

//        public static object ToObject(this Dictionary<string, object> dict, Type type)
//        {
//            var obj = Activator.CreateInstance(type);

//            foreach (var kv in dict)
//            {
//                if (kv.Value != null)
//                    SetValue(obj, kv.Key, kv.Value);
//            }
//            return obj;
//        }

//        public static T ToObject<T>(this Dictionary<string, object> dict)
//        {
//            Type type = typeof(T);
//            var obj = Activator.CreateInstance(type);

//            foreach (var kv in dict)
//            {
//                if (kv.Value != null)
//                    SetValue(obj, kv.Key, kv.Value);
//            }
//            return (T)obj;
//        }

//        public static IEnumerable GetPropertyValues(this IEnumerable obj, String name, Type type = null)
//        {
//            var info = (type ?? obj.FirstNG().GetType()).GetProperty(name);
//            foreach (var x in obj)
//                yield return GetPropValue(x, info);
//        }

//        public static object FirstNG(this IEnumerable enumerable)
//        {
//            IEnumerator enumerator = enumerable.GetEnumerator();
//            enumerator.MoveNext();
//            return enumerator.Current;
//        }

//        public static object GetPropValue(this Object obj, String name, Type type = null) => GetPropValue(obj, (type ?? obj.GetType()).GetProperty(name));

//        public static object GetPropValue(this Object obj, String name) =>
//            GetPropValue(obj, obj.GetType().GetProperty(name));

//        public static object GetPropValue(this Object obj, PropertyInfo info = null)
//        {
//            if (info == null) return null;
//            object retval = info.GetValue(obj, null);
//            return retval == null ? null : retval;
//        }

//        public static void SetValue(object inputObject, string propertyName, object propertyVal, bool ignoreCase = true)
//        {
//            System.Reflection.PropertyInfo propertyInfo = null;
//            //get the property information based on the
//            if (ignoreCase)
//                propertyInfo = inputObject.GetType().GetProperty(propertyName, BindingFlags.SetProperty |
//                       BindingFlags.IgnoreCase |
//                       BindingFlags.Public |
//                       BindingFlags.Instance);
//            else
//                propertyInfo = inputObject.GetType().GetProperty(propertyName);

//            //Convert.ChangeType does not handle conversion to nullable types
//            //if the property type is nullable, we need to get the underlying type of the property
//            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

//            //Returns an System.Object with the specified System.Type and whose value is
//            //equivalent to the specified object.
//            propertyVal = Convert.ChangeType(propertyVal, targetType);

//            //Set the value of the property
//            propertyInfo.SetValue(inputObject, propertyVal, null);

//        }

//        private static bool IsNullableType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));

//    }

//}