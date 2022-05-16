using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Common.Helper;
using Utility.WPF.Helper;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls.Buttons.Infrastructure
{
    public class CheckedRoutedEventArgs : RoutedEventArgs
    {
        public CheckedRoutedEventArgs(RoutedEvent routedEvent,
            object source,
            ICollection<ChangedItem> dictionary)
            : base(routedEvent, source)
        {
            Dictionary = dictionary;
        }

        public ICollection<ChangedItem> Dictionary { get; }

        public record struct ChangedItem(object Key, bool? Old, bool? New);
    }

    public static class CheckBoxesHelper
    {
        public static void Bind(FrameworkElement element, object item, object sender)
        {
            if (sender is not IIsCheckedPath checkedPath ||
                sender is not Selector selector)
            {
                throw new Exception("sdf4 fdgdgp;p;p");
            }

            BindingFactory factory = new(item);
            if (string.IsNullOrEmpty(checkedPath.IsCheckedPath) == false)
                element.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, factory.TwoWay(checkedPath.IsCheckedPath));


            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false)
            //{
            //    element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.SelectedValuePath));
            //}
            if (string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            {
                element.SetBinding(FrameworkElement.TagProperty, factory.OneWay(selector.DisplayMemberPath));
            }
            else
            {
                throw new Exception($"Expected " +
                    $"{nameof(Selector.DisplayMemberPath)} " +
                    $"not to be null.");
            }
        }

        public static Dictionary<object, bool?> ToDictionary(this System.Windows.Controls.Primitives.Selector selector)
        {
            //if (sender is not System.Windows.Controls.Primitives.Selector selector)
            //{
            //    throw new System.Exception("sdf4 fdgdgp;p;p");
            //}

            //if (string.IsNullOrEmpty(selector.SelectedValuePath) == false ||
            //    string.IsNullOrEmpty(selector.DisplayMemberPath) == false)
            //{
            var items = selector.ItemsOfType<CheckBox>().ToArray();
            var output = items.Where(a => a is { Tag: { } tag }).ToDictionary(a => a.Tag, a => a.IsChecked);
            return output;
            //}
        }
    }




    public class CheckProfile : AutoMapper.Profile
    {
        public CheckProfile()
        {
            CreateMap<CheckedRoutedEventArgs, Dictionary<object, bool?>>()
                .ConvertUsing(a => (a.Source as Selector).ToDictionary());
        }
    }

    class DifferenceHelper
    {
        private readonly Selector selector;
        private Dictionary<object, bool?>? dictionary;

        public DifferenceHelper(Selector selector)
        {
            this.selector = selector;
        }

        public IEnumerable<CheckedRoutedEventArgs.ChangedItem> Get
        {
            get
            {
                var tempDictionary = CheckBoxesHelper.ToDictionary(selector);
                var differences = Differences().ToArray();
                dictionary = tempDictionary;
                return differences;

                IEnumerable<CheckedRoutedEventArgs.ChangedItem> Differences()
                {
                    if (dictionary == null)
                    {
                        return Array.Empty<CheckedRoutedEventArgs.ChangedItem>();
                    }
                    return dictionary.Differences(tempDictionary)
                        .Select(a => new CheckedRoutedEventArgs.ChangedItem(a.key, a.one, a.two));
                }
            }
        }
    }
}
