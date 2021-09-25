using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityHelperEx;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls.Master
{

    using System.Data;
    using System.Windows.Controls.Primitives;
    using Mixins;
    using ReactiveUI;
    using UtilityWpf.Service;
    using fac = DependencyPropertyFactory<ReadOnlyMasterDetail>;
    using fac2 = DependencyPropertyFactory<MasterDetail>;

    /// <summary>
    /// Only transforms master-list items to the detail-item; and not vice-versa
    /// </summary>
    public class ReadOnlyMasterDetail : ContentControlx
    {

        public static readonly DependencyProperty ConverterProperty = fac.Register<IValueConverter>();
        public static readonly DependencyProperty ConverterParameterProperty = fac.Register<object>();
        public static readonly DependencyProperty PropertyKeyProperty = fac.Register<string>(nameof(PropertyKey));
        public static readonly DependencyProperty UseDataContextProperty = fac.Register<bool>();
        public static readonly DependencyProperty SelectorProperty = fac.Register<Control>();

        static ReadOnlyMasterDetail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReadOnlyMasterDetail), new FrameworkPropertyMetadata(typeof(ReadOnlyMasterDetail)));
        }

        public ReadOnlyMasterDetail()
        {
            this.WhenAnyValue(a => a.Selector).WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.DataContext).WhereNotNull())
                .Subscribe(a =>
                {
                    a.First.DataContext = a.Second;
                });

            TransformObservable = Transform(
                this.WhenAnyValue(a => a.Selector).WhereNotNull().Select(SelectFromMaster).Switch(),
                this.Observable<IValueConverter>(nameof(Converter)),
                this.Observable<object>(nameof(ConverterParameter)),
                this.Observable<string>(nameof(PropertyKey))).ToReplaySubject(0);

            _ = TransformObservable
                .Select(a => a.New)
                .WhereNotNull()
                .Subscribe(content =>
                {
                    SetDetail(Content, content);
                });
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

        #endregion properties

        protected IObservable<TransformProduct?> TransformObservable { get; }

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
            if (content is ContentControl contentControl)
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
            transforms
                 .WhereNotNull()
                 .Scan((default(TransformProduct), default(TransformProduct)), (a, b) => (a.Item2, b))
                 .Select(a => a.Item1)
                 .WhereNotNull()
                 .Where(a => a.Old != null)
                 .Subscribe(tp =>
                 {
                     object? replacement = null;

                     replacement = (tp.New, tp.Converter, tp.DataKey) switch
                     {
                         (null, _, _) => null,
                         (object o, IValueConverter conv, _) => conv.ConvertBack(o, default, tp.ConverterParameter, default),
                         (object o, _, string key) => PropertyConverter.ConvertBack(tp.Old!, key, o),
                         (object o, null, null) => o
                     };

                     if (replacement == null)
                         throw new Exception("7788dfdfgf");
                     //if (tp.Old != null && tp.Old == replacement && tp.Old != null)
                     //    throw new ApplicationException("selectedOld and ee can't be the same object in order to compare them after conversion.");

                     PropertyMerger.Instance.Set(tp.Old!, replacement);
                 });
        }
    }
}