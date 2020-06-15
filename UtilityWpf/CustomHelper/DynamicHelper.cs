using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UtilityHelper;
using UtilityHelper.NonGeneric;

namespace CustomHelper
{
    public static class DynmamicHelper
    {
        public static List<Dynamic> OnGetData(IEnumerable enumerable, string key, string value)
        {
            var keys = ((IEnumerable)enumerable.First()).GetPropertyValues<object>(key);
            //var values= ((IEnumerable)enumerable.First()).GetPropertyValues(value);

            try
            {
                foreach (var k in keys)
                    Dynamic.AddProperty((string)k, typeof(string));
            }
            catch
            {
            }
            var Customers = new List<Dynamic>();

            foreach (var en in enumerable)
            {
                var values = ((IEnumerable)en).GetPropertyValues<object>(value);
                Dynamic customer1 = new Dynamic();// { FirstName = "Julie", LastName = "Smith" };
                foreach (var val in values.Cast<object>().Zip(keys.Cast<object>(), (a, b) => new { a, b }))
                    customer1.SetPropertyValue((string)val.b, val.a);

                Customers.Add(customer1);
            }

            return Customers;
        }
    }
}