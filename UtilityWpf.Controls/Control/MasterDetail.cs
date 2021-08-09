using DynamicData;
using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UtilityHelper.NonGeneric;
using UtilityHelperEx;
using UtilityWpf.Abstract;
using UtilityWpf.Controls;

namespace UtilityWpf.Controls
{
    using static DependencyPropertyFactory<MasterDetail>;
    public class MasterDetail : ContentControlx
    {
        [Flags]
        public enum ButtonType
        {
            None = 0, Duplicate = 1, Delete = 2, Check = 4, All = Duplicate | Delete | Check
        }

        Subject<IEnumerable> ItemsSourceSubject = new();

        public static readonly DependencyProperty ItemsSourceProperty = Register(a => a.ItemsSourceSubject, nameof(ItemsSource));
        public static readonly DependencyProperty OutputProperty = DependencyHelper.Register<object>();
        public static readonly DependencyProperty DataConverterProperty = DependencyHelper.Register<IValueConverter>();
        public static readonly DependencyProperty DataKeyProperty = DependencyHelper.Register<string>();
        public static readonly DependencyProperty UseDataContextProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty RemoveOrderProperty = DependencyHelper.Register<ButtonType>();
        protected ListBox listBox;

        static MasterDetail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetail), new FrameworkPropertyMetadata(typeof(MasterDetail)));
        }

        public MasterDetail()
        {
        }

        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
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




        public override void OnApplyTemplate()
        {
            var selector = Template.Resources["propertytemplateSelector"] as DataTemplateSelector;
            Content ??= new ContentControl { ContentTemplateSelector = selector };

            listBox = GetTemplateChild("PART_List") as ListBox;
            if (listBox == null)
                return;

            ItemsSourceSubject
                .StartWith(ItemsSource)
                .Subscribe(a => listBox.ItemsSource = a);


            _ = listBox
              .SelectSelectionAddChanges()
              .Where(a => a.Count == 1)
              .Select(a => a.First())
              .CombineLatest(this.WhenAnyValue(a => a.DataConverter), this.WhenAnyValue(a => a.DataKey))
              .ObserveOnDispatcher()
              .Subscribe(a =>
              {
                  var (selected, converter, key) = a;
                  var content = selected;
                  if (converter != null)
                      content = converter.Convert(content, default, default, default);
                  if (key != null)
                      content = UtilityHelper.PropertyHelper.GetPropertyValue<object>(content, key);
                  SetContent(Content, content);
              });
        }

        protected virtual void SetContent(object content, object @object)
        {
            if (UseDataContext)
                if (content is FrameworkElement c)
                {
                    c.DataContext = @object;
                }
                else
                {
                    throw new Exception("Content needs to be framework element is UseDataContext set to true");
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
            else if (content is JsonView propertyGrid)
            {
                propertyGrid.Object = propertyGrid;
            }
            else if (content is ContentControl contentControl)
            {
                contentControl.Content = @object;
            }
            else
            {
                Content = @object;
            }

            // else throw new Exception(nameof(Content) + " needs to have property");
        }

        private class DefaultFilter : UtilityInterface.NonGeneric.IFilter
        {
            public bool Filter(object o)
            {
                return true;
            }
        }
    }
}