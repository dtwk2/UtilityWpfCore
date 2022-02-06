using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Evan.Wpf;
using ReactiveUI;
using UtilityHelper.NonGeneric;
using UtilityWpf.Abstract;

namespace UtilityWpf.Base
{
    /// <summary>
    /// A ContentControl for content deriving from ItemsControl
    /// </summary>
    public class SelectorAndContentControl : DoubleContentControl, ISelector
    {
        public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(SelectorAndContentControl));
        public static readonly DependencyProperty CountProperty = DependencyHelper.Register<int>();
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectorAndContentControl));

        static SelectorAndContentControl()
        {
        }

        public SelectorAndContentControl()
        {
        }

        #region properties

        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public virtual event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public virtual object SelectedItem
        {
            get
            {
                return this.Content switch
                {
                    Selector selector => selector.SelectedItem,
                    ISelector selector => selector.SelectedItem,
                    _ => throw new InvalidOperationException("F£FFF"),
                };
            }
        }

        public virtual int SelectedIndex
        {
            get
            {
                return this.Content switch
                {
                    Selector selector => selector.SelectedIndex,
                    ISelector selector => selector.SelectedIndex,
                    _ => -1,
                };
            }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            //this.SetValue(ItemsSourceProperty, itemsControl.ItemsSource);

            if (Content is Selector selector)
            {
                selector
                    .WhenAnyValue(c => c.ItemsSource)
                    .Subscribe(itemsSource => { this.ItemsSource ??= itemsSource; });
                selector.SelectionChanged += (_, e) =>
                {
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
                    this.SetValue(CountProperty, selector.Items.Count);
                };
                this.SetValue(CountProperty, selector.ItemsSource?.Count() ?? 0);
            }
            else if (Content is ISelector iSelector)
            {
                iSelector
                    .WhenAnyValue(c => c.ItemsSource)
                    .Subscribe(itemsSource => { this.ItemsSource ??= itemsSource; });
                iSelector.SelectionChanged += (_, e) =>
                {
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
                    this.SetValue(CountProperty, iSelector.ItemsSource?.Count() ?? 0);
                };
                this.SetValue(CountProperty, iSelector.ItemsSource?.Count() ?? 0);
            }
            //else if (Content is ItemsControl itemsControl)
            //{
            //    if (Content is ItemsControl { ItemsSource: INotifyCollectionChanged changed })
            //    {
            //        changed.CollectionChanged += (s, e) =>
            //        {
            //            this.SetValue(CountProperty, (changed as IEnumerable)?.Count() ?? -1);
            //        };
            //    }
            //    itemsControl
            //        .WhenAnyValue(c => c.ItemsSource)
            //        .Subscribe(itemsSource => { this.ItemsSource ??= itemsSource; });

            //    itemsControl
            //        .WhenAnyValue(a => a.ItemsSource)
            //        .WhereNotNull()
            //        .DistinctUntilChanged()
            //        .Subscribe(iSource =>
            //        {
            //            if (iSource is INotifyCollectionChanged collectionChanged)
            //            {
            //                collectionChanged.CollectionChanged += (s, e) =>
            //                {
            //                    this.SetValue(CountProperty, iSource.Count());
            //                };

            //                Count = iSource.Count();
            //            }
            //            // ItemsSource = iSource;
            //        });
            //}
            else
            {
                throw new Exception("sdfsd3311.3 ");
            }
            Count = ItemsSource?.Count() ?? 0;

            this.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    if (Content is ItemsControl itemsControl)
                        itemsControl.ItemsSource = a;

                });

            base.OnApplyTemplate();
        }
    }
}