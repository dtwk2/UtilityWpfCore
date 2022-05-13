﻿using NetFabric.Hyperlinq;
using Soukoku.ExpressionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Helper;
using UtilityHelper;
using UtilityHelper.NonGeneric;

namespace UtilityWpf
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class CountToBooleanConverter : IValueConverter
    {
        private Lazy<Evaluator> lazy = new(() =>
         {
             var evaluator = new Evaluator(new EvaluationContext());
             return evaluator;
         });

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int param;
            if (int.TryParse(parameter.ToString(), out param))
            {
                if (GetInt(value) is int i)
                    return (i >= param) != Invert;
            }

            if (ExpressionCharacters(parameter.ToString() ?? string.Empty).Any())
                if (GetInt(value) is var i2)
                {
                    var evaluate = lazy.Value.Evaluate($"{i2}{parameter}").Value;
                    var result = int.Parse(evaluate) == 1;
                    return result != Invert;
                }

            return DependencyProperty.UnsetValue;

            static int? GetInt(object value)
            {
                if (value.GetType().IsNumericType())
                {
                    return (int)value;
                }
                if (value is DependencyObject ui)
                {
                    if (ui is ItemsControl ic)
                    {
                        return ic.ItemsSource?.Count();
                    }
                    var itemsControl = ui.ChildOfType<ItemsControl>();
                    return itemsControl?.ItemsSource?.Count();
                }
                if (value.ToString() is var str)
                    if (int.TryParse(str, out var pValue))
                    {
                        return pValue;
                    }

                return null;
            }

            static IEnumerable<char> ExpressionCharacters(string value)
            {
                return from chr in value.ToCharArray()
                       join exp in expressionCharacters
                       on chr equals exp
                       select chr;
            }
        }

        private static char[] expressionCharacters = new char[] {
                    '+', //addition,
                    '-', //subtraction / negative,
                    '*', //multiplication,
                    '/', //division,
                    '%', //modulus,
                    '&', //bitwise and,
                    '|', //bitwise or,
                    '!', //logical negation,
                    //'&&', //logical and,
                    //'||', //logical or,
                    '<', //less than,
                    //'<=', //less than or equals,
                    '>', //greater than,
                         //'>=', //greater than or equals,
                         //'==', //equals,
                         //'!=', //not equals,
                         //'++', //preincrement,
                         //'--', //predecrement,
            };

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public bool Invert { get; set; }
    }
}