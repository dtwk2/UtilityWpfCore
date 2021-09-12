﻿using System;
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
    using System.ComponentModel;
    using System.Windows.Controls.Primitives;
    using Mixins;
    using ReactiveUI;
    using UtilityWpf.Service;
    using static DependencyPropertyFactory<MasterDetail>;

    public class MasterDetail : ContentControlx
    {

        public static readonly DependencyProperty DataConverterProperty = Register<IValueConverter>();
        public static readonly DependencyProperty DataKeyProperty = Register<string>(nameof(DataKey));
        public static readonly DependencyProperty UseDataContextProperty = Register<bool>();
        public static readonly DependencyProperty SelectorProperty = Register<Control>();

        static MasterDetail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetail), new FrameworkPropertyMetadata(typeof(MasterDetail)));
        }

        public MasterDetail()
        {
            _ = SelectContent()
                .Subscribe(content =>
                {
                    SetContent(Content, content);
                });
        }

        #region properties
        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public Control Selector
        {
            get { return (Control)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        public string DataKey
        {
            get { return (string)GetValue(DataKeyProperty); }
            set { SetValue(DataKeyProperty, value); }
        }

        public bool UseDataContext
        {
            get { return (bool)GetValue(UseDataContextProperty); }
            set { SetValue(UseDataContextProperty, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            // var selector = Template.Resources["propertytemplateSelector"] as DataTemplateSelector;
            //Content ??= new ContentControl { ContentTemplateSelector = selector };
            base.OnApplyTemplate();
        }

        protected virtual IObservable<object> SelectContent()
        {
            var transform = Transform(SelectChanges(),
                this.Observable<IValueConverter>(nameof(DataConverter)),
                this.Observable<string>(nameof(DataKey))).ToReplaySubject(0);

            transform
                .Select(a => a.Item2)
                .Where(a =>
                {
                    // either they are the same object or same type but don't equal
                    return a.Item2 != null && (a.Item1 == a.Item2 || (a.Item1?.Equals(a.Item2) == false));
                })
                .CombineLatest(SelectItemsSource())
                .Subscribe(a =>
                {

                    if (a.Second is IList { IsReadOnly: false, IsFixedSize: false } list)
                    {
                        int index = list.IndexOf(a.First.Item1);
                        if (index < 0)
                        {
                            return;
                        }
                        list.RemoveAt(index);
                        list.Insert(index, a.First.Item2);
                        return;
                    }


                    var first = a.Item1;
                    if (first.Item1 is not INotifyPropertyChanged)
                    {
                        MessageBox.Show("object does not implement INotifyPropertyChanged. Therefore change will not be noticed by subscribers and won't be automatically persisted!");
                    }
                    PropertyMerger.Instance.Set(first.Item1!, first.Item2!);
                });

            return transform
                .Select(a => a.Item1);
        }

        private IObservable<object> SelectChanges()
        {
            return this.WhenAnyValue(a => a.Selector)
                .WhereNotNull()
                .Select(a =>
                {
                    if (Selector?.Name == "dsfd")
                    {

                    }
                    (Selector as Selector)
                        ?.SelectSingleSelectionChanges()
                        .Subscribe(a =>
                        {

                        });

                    return a switch
                    {
                        ISelector selector => selector.SelectSingleSelectionChanges(),
                        Selector selector => selector.SelectSingleSelectionChanges(),
                        _ => throw new ApplicationException($"Unexpected type,{a.GetType().Name} for {nameof(Selector)} "),
                    };
                }).Switch();
        }

        private void MasterDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private IObservable<IEnumerable?> SelectItemsSource()
        {
            return this.Observable<Control>()
                .CombineLatest(this.LoadedChanges(), (a, b) => a)
                        .SelectMany(a =>
                {
                    return a switch
                    {
                        ISelector selector => selector.WhenAnyValue(a => a.ItemsSource).StartWith(selector.ItemsSource),
                        Selector selector => selector.WhenAnyValue(a => a.ItemsSource),
                        _ => throw new ApplicationException($"Unexpected type,{a.GetType().Name} for {nameof(Selector)} "),
                    };
                });
        }

        protected virtual void SetContent(object content, object @object)
        {
            if (UseDataContext)
            {
                if (content is FrameworkElement frameworkElement)
                {
                    frameworkElement.DataContext = @object;
                }
                else
                {
                    throw new ApplicationException("Content needs to be framework element is UseDataContext set to true");
                }
            }
            else if (@object is IEnumerable enumerable)
            {
                if (content is IItemsSource oview)
                {
                    oview.ItemsSource = enumerable;
                }
                else if (content is ItemsControl itemsControl)
                {
                    itemsControl.ItemsSource = enumerable;
                }
            }
            //else if (content is JsonView propertyGrid)
            //{
            //    propertyGrid.Object = propertyGrid;
            //}
            else if (content is ContentControl contentControl)
            {
                contentControl.Content = @object;
            }
            else
            {
                Content = @object;
            }
        }

        private class DefaultFilter : UtilityInterface.NonGeneric.IFilter
        {
            public bool Filter(object o)
            {
                return true;
            }
        }

        protected static IObservable<(object newItem, (object? old, object? replacement) change)> Transform(IObservable<object> collectionViewModel, IObservable<IValueConverter> dataConversions, IObservable<string> dataKeys)
        {
            collectionViewModel
                .Subscribe(a =>
                {

                });
            return ObservableEx
                .CombineLatest(collectionViewModel, dataConversions, dataKeys)
                .ObserveOnDispatcher()
                .Scan(default((object, object, object?, object?, IValueConverter?, string?)), (a, b) =>
             {

                 var (selectedOld, conversion, ssOld, eeOld, converterOld, dataKeyOld) = a;
                 var (selected, converter, dataKey) = b;


                 var ee = (conversion, converterOld, dataKeyOld) switch
                 {
                     (null, _, _) => null,
                     (object o, IValueConverter conv, _) => null/*Catch(a=> conv.ConvertBack(o, default, default, default))*/,
                     (object o, _, string key) => ConvertBack(selectedOld, key, o),
                     //(null,null) => throw new Exception($"Either {nameof(DataConverter)} or {nameof(DataKey)} must be set if {nameof(ItemsSource)} set")
                     (object o, null, null) => o
                 };


                 var ss = (converter, dataKey) switch
                 {
                     (IValueConverter conv, _) => conv.Convert(selected, default, default, default),
                     (_, string key) => UtilityHelper.PropertyHelper.GetPropertyValue<object>(selected, key),
                     //(null,null) => throw new Exception($"Either {nameof(DataConverter)} or {nameof(DataKey)} must be set if {nameof(ItemsSource)} set")
                     (null, null) => selected
                 };

                 if (selectedOld != null && selectedOld == ee && converterOld != null)
                 {
                     throw new ApplicationException("selectedOld and ee can't be the same object in order to compare them after conversion.");
                 }
                 return (selected, ss, selectedOld, ee, converter, dataKey);
             })
                .Select(a => (a.Item2, (a.Item3, a.Item4)));

            static object ConvertBack(object selected, string k, object selectedValueOld)
            {
                UtilityHelper.PropertyHelper.SetValue(selected, k, selectedValueOld);
                return selectedValueOld;
            }
        }


        protected static IObservable<object> Transform2(IObservable<object> collectionViewModel, IObservable<IValueConverter> dataConversions, IObservable<string> dataKeys)
        {
            collectionViewModel
                .Subscribe(a =>
                {

                });
            return ObservableEx
                .CombineLatest(collectionViewModel, dataConversions, dataKeys)
                .ObserveOnDispatcher()
                .Scan(default((object, object, IValueConverter?, string?)), (a, b) =>
             {

                 var (selected, converter, dataKey) = b;

                 var ss = (converter, dataKey) switch
                 {
                     (IValueConverter conv, _) => conv.Convert(selected, default, default, default),
                     (_, string key) => UtilityHelper.PropertyHelper.GetPropertyValue<object>(selected, key),
                     //(null,null) => throw new Exception($"Either {nameof(DataConverter)} or {nameof(DataKey)} must be set if {nameof(ItemsSource)} set")
                     (null, null) => selected
                 };

                 return (selected, ss, converter, dataKey);
             })
                .Select(a => (a.Item2));

        }
    }
}