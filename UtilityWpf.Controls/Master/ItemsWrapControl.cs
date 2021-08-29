
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Evan.Wpf;
using ReactiveUI;
using System.Collections.Specialized;
using UtilityHelper.NonGeneric;
using UtilityWpf.Abstract;
using UtilityWpf.Mixins;
using System.Reactive.Subjects;

namespace UtilityWpf.Controls
{

    public class WrapControl : ItemsWrapControl
    {
        public static readonly DependencyProperty ControlsCollectionProperty =
            DependencyProperty.Register("ControlsCollection", typeof(IEnumerable), typeof(WrapControl), new PropertyMetadata(null));

        static WrapControl()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(WrapControl), new FrameworkPropertyMetadata(typeof(WrapControl)));
        }


        public WrapControl()
        {
            //this.Template = new ControlTemplate();

            this.Observable<IEnumerable>(nameof(ControlsCollection)).Subscribe(a =>
            {

            });

            this.Observable<IEnumerable>(nameof(ControlsCollection)).CombineLatest(wrapPanelSubject)
                .Where(a => a.First != null && a.Second != null)
                .Take(1)
                .Subscribe(a =>
                {
                    var (collection, wrapPanel) = a;
                    wrapPanel.Children.Clear();
                    foreach (var item in ControlsCollection.OfType<UIElement>())
                    {
                        wrapPanel.Children.Add(item);
                    }
                });
        }


        public IEnumerable ControlsCollection
        {
            get { return (IEnumerable)GetValue(ControlsCollectionProperty); }
            set { SetValue(ControlsCollectionProperty, value); }
        }


    }

    public class ItemsWrapControl : ContentControlx, ISelector
    {
        private Selector Selector => ItemsControl is Selector selector ? selector : throw new Exception($@"The ItemsControl used must be of type {nameof(Selector)} for operation.");

        public static readonly DependencyProperty OrientationProperty = DependencyHelper.Register<Orientation>(new PropertyMetadata(Orientation.Horizontal));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsWrapControl), new PropertyMetadata(null));
        private static readonly DependencyProperty ItemsControlProperty = DependencyProperty.Register("ItemsControl", typeof(ItemsControl), typeof(ItemsWrapControl), new PropertyMetadata(null));
        public static readonly DependencyProperty CountProperty = DependencyHelper.Register<int>();
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(ItemsWrapControl));
        protected readonly ReplaySubject<WrapPanel> wrapPanelSubject = new(1);

        static ItemsWrapControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsWrapControl), new FrameworkPropertyMetadata(typeof(ItemsWrapControl)));
        }

        public ItemsWrapControl()
        {
            this.Control<WrapPanel>().Subscribe(wrapPanelSubject);

            wrapPanelSubject.Subscribe(a =>
            {

            });

            this.WhenAnyValue(a => a.Orientation)
                .CombineLatest(wrapPanelSubject)
                .Where(a => a.Second != null)
                .Subscribe(c =>
                {
                    var (orientation, wrapPanel) = c;

                    wrapPanel.DataContext = ItemsControl?.ItemsSource;

                    if (orientation == Orientation.Horizontal)
                    {
                        DockPanel.SetDock(wrapPanel, Dock.Right);
                        wrapPanel.Orientation = Orientation.Vertical;
                    }
                    else if (orientation == Orientation.Vertical)
                    {
                        DockPanel.SetDock(wrapPanel, Dock.Bottom);
                        wrapPanel.Orientation = Orientation.Horizontal;
                    }
                });
        }

        #region properties

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
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
            var dockPanel = this.GetTemplateChild("PART_DockPanel") as DockPanel;
            var wrapPanel = this.GetTemplateChild("PART_WrapPanel") as WrapPanel;
            wrapPanelSubject.OnNext(wrapPanel);
            if (dockPanel == null)
                throw new ApplicationException();
            if (wrapPanel == null)
                throw new ApplicationException();


            ItemsControl = (this.Content as ItemsControl) ?? (this.Content as DependencyObject)?.FindVisualChildren<ItemsControl>().SingleOrDefault()!;
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
                    this.SetValue(CountProperty, ItemsControl.ItemsSource.Count());
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

