using Evan.Wpf;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Utility;

namespace UtilityWpf.Controls
{
    internal class HeaderedItemsControlEx : DependencyObject
    {
        public static readonly DependencyProperty ShowCountInHeaderProperty = DependencyHelper.Register<bool>();

        public static IObservable<object> HeaderChanges(HeaderedItemsControl headeredItemsControl, IObservable<bool> showCountInHeader)
        {
            return showCountInHeader
                .CombineLatest(
                headeredItemsControl.WhenAnyValue(a => a.Header).WhereNotNull().DistinctUntilChanged(), headeredItemsControl.CountChanges())
                .Select(a =>
                {
                    var header = Regex.Replace(a.Second?.ToString() ?? string.Empty, @"(([\w\d\s]*) ?(\(\d*\))?)", new MatchEvaluator(match =>
                    {
                        if (match.Groups[0].Value == (a.Second?.ToString() ?? string.Empty))
                            return match.Groups[2].Value.TrimEnd();
                        else if (match.Groups[0].Value == string.Empty)
                            return a.First ? $" ({a.Third})" : string.Empty;
                        return string.Empty;
                    }));

                    return header;
                });
        }
    }

    public class HeaderedDataGrid : HeaderedItemsControl
    {
        public static readonly DependencyProperty ShowCountInHeaderProperty = HeaderedItemsControlEx.ShowCountInHeaderProperty.AddOwner(typeof(HeaderedDataGrid), new PropertyMetadata(true));

        static HeaderedDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedDataGrid), new FrameworkPropertyMetadata(typeof(HeaderedDataGrid)));
        }

        public HeaderedDataGrid()
        {
            HeaderedItemsControlEx.HeaderChanges(this, this.WhenAnyValue(a => a.ShowCountInHeader))
            .Subscribe(a => Header = a);
        }

        public bool ShowCountInHeader
        {
            get { return (bool)GetValue(ShowCountInHeaderProperty); }
            set { SetValue(ShowCountInHeaderProperty, value); }
        }
    }
}