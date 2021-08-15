using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityHelper.NonGeneric;
using UtilityHelperEx;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls
{
    using Mixins;

    using static DependencyPropertyFactory<MasterDetail>;
    public class MasterDetail : ContentControlx
    {
        [Flags]
        public enum ButtonType
        {
            None = 0, Duplicate = 1, Delete = 2, Check = 4, All = Duplicate | Delete | Check
        }

        //protected Subject<IEnumerable> ItemsSourceSubject = new();

        public static readonly DependencyProperty ItemsSourceProperty = Register(nameof(ItemsSource));
        public static readonly DependencyProperty OutputProperty = Register<object>();
        public static readonly DependencyProperty DataConverterProperty = Register<IValueConverter>();
        public static readonly DependencyProperty DataKeyProperty = Register<string>(nameof(DataKey));
        public static readonly DependencyProperty UseDataContextProperty = Register<bool>();
        //public static readonly DependencyProperty RemoveOrderProperty = Register<ButtonType>();
        protected ListBox? listBox;

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

            _ = SelectContent()
            .Subscribe(content =>
            {
                SetContent(Content, content);
            });


            //if ((listBox = GetTemplateChild("PART_List") as ListBox) == null)
            //    return;
            this.Control<ListBox>().Subscribe(a => { 
            
            });

            this.Observable<IEnumerable>()
               .CombineLatest(this.Control<ListBox>())
                .Subscribe(a => a.Second.ItemsSource = a.First);

            base.OnApplyTemplate();

        }


        protected virtual IObservable<object> SelectContent()
        {
            return Transform(this.Control<ListBox>().SelectMany(a => a.SelectSingleSelectionChanges()),
                this.Observable<IValueConverter>(nameof(DataConverter)),
                this.Observable<string>(nameof(DataKey)));
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


        protected static IObservable<object> Transform(IObservable<object> collectionViewModel, IObservable<IValueConverter> dataConversions, IObservable<string> dataKeys)
        {
            collectionViewModel.Subscribe(a =>
            {

            });
            return ObservableEx
                .CombineLatest(collectionViewModel, dataConversions, dataKeys)
                .ObserveOnDispatcher()
                .Select(a =>
                {
                    var (selected, converter, dataKey) = a;
                    return (converter, dataKey) switch
                    {
                        (IValueConverter conv, _) => conv.Convert(selected, default, default, default),
                        (_, string k) => UtilityHelper.PropertyHelper.GetPropertyValue<object>(selected, k),
                        //(null,null) => throw new Exception($"Either {nameof(DataConverter)} or {nameof(DataKey)} must be set if {nameof(ItemsSource)} set")
                        (null, null) => selected
                    };
                });
        }
    }
}