using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls.Master
{
    using Evan.Wpf;
    using ReactiveUI;
    using System.Reactive.Subjects;
    using System.Windows.Controls.Primitives;
    using UtilityWpf.Mixins;
    using fac = DependencyPropertyFactory<ReadOnlyMasterDetail>;

    /// <summary>
    /// Only transforms master-list items to the detail-item; and not vice-versa
    /// </summary>
    public class ReadOnlyMasterDetail : ContentControlx, ISelector
    {
        public static readonly DependencyProperty ConverterProperty = fac.Register<IValueConverter>();
        public static readonly DependencyProperty ConverterParameterProperty = fac.Register<object>();
        public static readonly DependencyProperty PropertyKeyProperty = fac.Register<string>(nameof(PropertyKey));
        public static readonly DependencyProperty UseDataContextProperty = fac.Register<bool>();
        public static readonly DependencyProperty SelectorProperty = DependencyHelper.Register(new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty OrientationProperty = DependencyHelper.Register(new PropertyMetadata(Orientation.Horizontal, Changed));

        private ReplaySubject<Control> controlSubject = new(1);
        private ReplaySubject<object> dataContextSubject = new(1);
        private ReplaySubject<Orientation> orientationSubject = new(1);

        static ReadOnlyMasterDetail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReadOnlyMasterDetail), new FrameworkPropertyMetadata(typeof(ReadOnlyMasterDetail)));
            DataContextProperty.OverrideMetadata(typeof(ReadOnlyMasterDetail), new FrameworkPropertyMetadata(null, Changed));
        }

        public ReadOnlyMasterDetail()
        {
            _ = controlSubject
                 .WhereNotNull()
                 .CombineLatest(dataContextSubject.WhereNotNull())
                 .Subscribe(a =>
                 {
                     a.First.DataContext = a.Second;
                 });

            TransformObservable = UtilityHelperEx.ObservableHelper.ToReplaySubject(Transform(
                controlSubject.ObserveOnDispatcher().WhereNotNull().Select(SelectFromMaster).Switch(),
                this.Observable<IValueConverter>(nameof(Converter)),
                this.Observable<object>(nameof(ConverterParameter)),
                this.Observable<string>(nameof(PropertyKey))));

            _ = TransformObservable
                .Select(a => a.New)
                .WhereNotNull()
                .Subscribe(content =>
                {
                    SetDetail(Content, content);
                });

            _ = orientationSubject
                .CombineLatest(controlSubject)
                .Subscribe(combined =>
                {
                    if (combined.Second is IOrientation iori)
                    {
                        iori.Orientation = (Orientation)(((int)combined.First + 1) % ((int)Orientation.Vertical + 1));
                    }
                });
        }

        event SelectionChangedEventHandler ISelector.SelectionChanged
        {
            add
            {
                switch (Selector)
                {
                    case ISelector selector:
                        selector.SelectionChanged += value;
                        break;

                    case Selector selector:
                        selector.SelectionChanged += value;
                        break;

                    default: throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} ");
                };
            }

            remove
            {
                switch (Selector)
                {
                    case ISelector selector:
                        selector.SelectionChanged -= value;
                        break;

                    case Selector selector:
                        selector.SelectionChanged -= value;
                        break;

                    default: throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} ");
                };
            }
        }

        #region properties

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        public Control Selector
        {
            get { return (Control)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        public string PropertyKey
        {
            get { return (string)GetValue(PropertyKeyProperty); }
            set { SetValue(PropertyKeyProperty, value); }
        }

        public bool UseDataContext
        {
            get { return (bool)GetValue(UseDataContextProperty); }
            set { SetValue(UseDataContextProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion properties

        protected IObservable<TransformProduct?> TransformObservable { get; }

        object ISelector.SelectedItem
        {
            get
            {
                return Selector switch
                {
                    ISelector selector => selector.SelectedItem,
                    Selector selector => selector.SelectedItem,
                    _ => throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} "),
                };
            }
        }

        int ISelector.SelectedIndex
        {
            get
            {
                return Selector switch
                {
                    ISelector selector => selector.SelectedIndex,
                    Selector selector => selector.SelectedIndex,
                    _ => throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} "),
                };
            }
        }

        IEnumerable ISelector.ItemsSource
        {
            get
            {
                return Selector switch
                {
                    ISelector selector => selector.ItemsSource,
                    Selector selector => selector.ItemsSource,
                    _ => throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} "),
                };
            }
        }

        // Seemingly necessary to avoid use of WhenAnyValue for certain properties to avoid threading issues
        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ReadOnlyMasterDetail readOnly)
                return;

            if (e.NewValue is Control control)
                d.Dispatcher.InvokeAsync(() => readOnly.controlSubject.OnNext(control));
            else if (e.NewValue is object obj)
                d.Dispatcher.InvokeAsync(() => readOnly.dataContextSubject.OnNext(obj));
            else if (e.NewValue is Orientation orientation)
                d.Dispatcher.InvokeAsync(() => readOnly.orientationSubject.OnNext(orientation));
        }

        /// <summary>
        /// Gets the selections made in the master-list
        /// </summary>
        protected virtual IObservable<object> SelectFromMaster(Control slctr)
        {
            return slctr switch
            {
                ISelector selector => selector.SelectSingleSelectionChanges(),
                Selector selector => selector.SelectSingleSelectionChanges(),
                _ => throw new ApplicationException($"Unexpected type,{slctr.GetType().Name} for {nameof(Selector)} "),
            };
        }

        /// <summary>
        /// Updates the detail-item with changes made to the  master-list
        /// </summary>
        protected virtual void SetDetail(object content, object @object)
        {
            if (UseDataContext)
            {
                if (content is FrameworkElement frameworkElement)
                {
                    frameworkElement.DataContext = @object;
                    return;
                }
                else
                {
                    throw new ApplicationException("Content needs to be framework element is UseDataContext set to true");
                }
            }
            if (@object is IEnumerable enumerable)
            {
                if (content is IItemsSource oview)
                {
                    oview.ItemsSource = enumerable;
                    return;
                }
                else if (content is ItemsControl itemsControl)
                {
                    itemsControl.ItemsSource = enumerable;
                    return;
                }
            }
            //else if (content is JsonView propertyGrid)
            //{
            //    propertyGrid.Object = propertyGrid;
            //}
            if (content is ContentControl contentControl && @object is not ContentControl _)
            {
                contentControl.Content = @object;
                return;
            }
            else
            {
                Content = @object;
            }
        }

        protected record TransformProduct(object? New, object? Old, IValueConverter? Converter, object ConverterParameter, string? DataKey);

        /// <summary>
        /// Makes any changes to selected-item before becoming the detail item
        /// </summary>
        protected static IObservable<TransformProduct?> Transform(
            IObservable<object> collectionViewModel,
            IObservable<IValueConverter> dataConversions,
            IObservable<object> converterParameters,
            IObservable<string> dataKeys)
        {
            return ObservableEx
                .CombineLatest(collectionViewModel, dataConversions, converterParameters, dataKeys)
                .ObserveOnDispatcher()
                .Scan(default(TransformProduct), (a, b) =>
                {
                    var (selected, converter, converterParameter, dataKey) = b;

                    if (selected == null)
                        throw new Exception("ds009fsd");
                    object? newItem = Convert(selected, converter, converterParameter, dataKey);

                    return new TransformProduct(newItem, selected, converter, converterParameter, dataKey);
                });
        }

        protected static object? Convert(object selected, IValueConverter? converter, object? converterParameter, string? dataKey)
        {
            return (converter, dataKey) switch
            {
                (IValueConverter conv, _) => conv.Convert(selected, default, converterParameter, default),
                (_, string key) => PropertyConverter.Convert(selected, key),
                (null, null) => selected
            };
        }

        protected class PropertyConverter
        {
            public static object Convert(object selected, string key)
            {
                return UtilityHelper.PropertyHelper.GetPropertyRefValue<object>(selected, key);
            }

            public static object ConvertBack(object selected, string k, object selectedValueOld)
            {
                UtilityHelper.PropertyHelper.SetValue(selected, k, selectedValueOld);
                return selectedValueOld;
            }
        }

        //protected class DefaultFilter : UtilityInterface.NonGeneric.IFilter
        //{
        //    public bool Filter(object o)
        //    {
        //        return true;
        //    }
        //}
    }

    public class MasterDetail : ReadOnlyMasterDetail
    {
        static MasterDetail()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetail), new FrameworkPropertyMetadata(typeof(MasterDetail)));
        }

        public MasterDetail()
        {
            Transform(TransformObservable);
        }

        protected static void Transform(IObservable<TransformProduct?> transforms)
        {
            //transforms
            //     .WhereNotNull()
            //     .Scan((default(TransformProduct), default(TransformProduct)), (a, b) => (a.Item2, b))
            //     .Select(a => a.Item1)
            //     .WhereNotNull()
            //     .Where(a => a.Old != null)
            //     .Subscribe(tp =>
            //     {
            //         object? replacement = null;

            //         replacement = (tp.New, tp.Converter, tp.DataKey) switch
            //         {
            //             (null, _, _) => null,
            //             (object o, IValueConverter conv, _) => conv.ConvertBack(o, default, tp.ConverterParameter, default),
            //             (object o, _, string key) => PropertyConverter.ConvertBack(tp.Old!, key, o),
            //             (object o, null, null) => o
            //         };

            //         if (replacement == null)
            //             throw new Exception("7788dfdfgf");
            //         //if (tp.Old != null && tp.Old == replacement && tp.Old != null)
            //         //    throw new ApplicationException("selectedOld and ee can't be the same object in order to compare them after conversion.");

            //         PropertyMerger.Instance.Set(tp.Old!, replacement);
            //     });
        }
    }
}