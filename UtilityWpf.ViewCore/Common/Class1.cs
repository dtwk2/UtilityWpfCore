# nullable enable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using static LambdaConverters.ValueConverter;
using static LambdaConverters.TemplateSelector;


namespace UtilityWpf.View
{

    internal static class Converters
    {
        private static readonly Lazy<Dictionary<int, Color>> NiceColors = new Lazy<Dictionary<int, Color>>(() =>
          ColorStore.Collection
           .Select((a, i) => Tuple.Create(i, (Color)ColorConverter.ConvertFromString(a.Value)))
           .ToDictionary(a => a.Item1, a => a.Item2));


        public static IValueConverter JTokenToColorConverter() => Create<JTokenType, Color>(a => NiceColors.Value[(byte)a.Value]);

        public static IValueConverter ComplexPropertyMethodToValueConverter() => Create<string, IEnumerable<JToken>, string>(args =>

        ((IEnumerable<JToken>)args.Value?
             .GetType()
             .GetMethod(args.Parameter, new Type[0])
             .Invoke(args.Value, new object[0]))
             .First()
             .Children()
         );

        public static IValueConverter JArrayLengthConverter() => Create<JToken, string>(jToken => jToken.Value.Type switch
        {
            JTokenType.Array => $"[{jToken.Value.Children().Count()}]",
            JTokenType.Property => $"[ { jToken.Value.Children().FirstOrDefault().Children().Count()} ]",
            _ => throw new Exception("Type should be JProperty or JArray"),
        });

        public static IValueConverter JValueConverter() => Create<JValue, object?>(jval => jval.Value.Type switch
        {
            JTokenType.Null => (object?)"Null",
            _ => (object?)jval.Value.Value
        });

        //public IValueConverter JValueTypeToColorConverter() => Create<JValue, Color>(a => NiceColors.Value[(byte)a.Value.Type]);
        public static IValueConverter MethodToValueConverter() => Create<object?, object?, string>(a =>
        a.Value != null &&
            a.Parameter != null &&
            a.Value.GetType().GetMethod(a.Parameter, new Type[0]) is { } methodInfo ?
            methodInfo.Invoke(a.Value, new object[0]) :
            null
        );
    }


}
