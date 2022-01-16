using Evan.Wpf;
using NetFabric.Hyperlinq;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UtilityHelper.NonGeneric;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls.Master
{
    /// <summary>
    /// A ContentControl for content deriving from ItemsControl
    /// </summary>
    public class ItemsContentControl : DoubleContentControl, ISelector
    {
        private Selector Selector => ItemsControl is Selector selector ? selector : throw new Exception($@"The ItemsControl used must be of type {nameof(Selector)} for operation.");

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsContentControl), new PropertyMetadata(null));

        private static readonly DependencyProperty ItemsControlProperty = DependencyProperty.Register("ItemsControl", typeof(ItemsControl), typeof(ItemsContentControl), new PropertyMetadata(null));
        public static readonly DependencyProperty CountProperty = DependencyHelper.Register<int>();
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(ItemsContentControl));

        static ItemsContentControl()
        {
        }

        public ItemsContentControl()
        {
        }

        #region properties

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            private set { SetValue(ItemsControlProperty, value); }
        }

        public virtual object? SelectedItem
        {
            get
            {
                return ItemsControl switch
                {
                    Selector selector => selector.SelectedItem,
                    ISelector selector => selector.SelectedItem,
                    _ => null,
                };
            }
        }

        public virtual int SelectedIndex
        {
            get
            {
                return ItemsControl switch
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
            ItemsControl = (this.Content as ItemsControl) ?? (this.Content as DependencyObject)?.FindVisualChildren<ItemsControl>().SingleOrDefault();
            if (ItemsControl != null)
            {
                //this.SetValue(ItemsSourceProperty, itemsControl.ItemsSource);
                ItemsControl.WhenAnyValue(c => c.ItemsSource)

                    .Subscribe(itemsSource =>
                    {
                        if (this.ItemsSource == null)
                            this.ItemsSource = itemsSource;
                    });
            }
            else if (this.Content == null)
            {
                throw new Exception($"Content is null.");
            }
            else
                throw new Exception($"Expected content to derive from type of {nameof(ItemsControl)}.");

            if (ItemsControl is ISelector selectionChanged)
            {
                selectionChanged.SelectionChanged += (s, e) =>
                {
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
                    //this.SetValue(MasterControl.CountProperty, selectionChanged.ItemsSource.Count());
                };
            }
            // Unlikely to be both ISelectionChanged and Selector
            if (ItemsControl is Selector selector)
            {
                selector.SelectionChanged += (s, e) =>
                {
                    this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
                    this.SetValue(CountProperty, selector.ItemsSource.Count());
                };
                this.SetValue(CountProperty, selector.ItemsSource?.Count() ?? 0);
            }
            else if (ItemsControl.ItemsSource is INotifyCollectionChanged changed)
            {
                changed.CollectionChanged += (s, e) =>
                {
                    this.SetValue(CountProperty, ItemsControl.ItemsSource?.Count() ?? -1);
                };
            }
            else
            {
                ItemsControl
                    .WhenAnyValue(a => a.ItemsSource)
                    .WhereNotNull()
                    .DistinctUntilChanged()
                    .Subscribe(iSource =>
                    {
                        if (iSource is INotifyCollectionChanged changed)
                        {
                            changed.CollectionChanged += (s, e) =>
                            {
                                this.SetValue(CountProperty, iSource.Count());
                            };

                            Count = iSource.Count();
                        }
                        // ItemsSource = iSource;
                    });
            }
            Count = ItemsSource?.Count() ?? 0;

            this.WhenAnyValue(a => a.ItemsSource)
                .Subscribe(a =>
                {
                    if (a != null)
                        ItemsControl.ItemsSource = a;
                });

            base.OnApplyTemplate();
        }
    }
}