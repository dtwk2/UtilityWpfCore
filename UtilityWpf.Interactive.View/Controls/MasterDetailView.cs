using DynamicData;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UtilityHelperEx;
using UtilityWpf.Abstract;
using UtilityWpf.View;

namespace UtilityWpf.Interactive.View.Controls
{
    public class MasterDetailView : ContentControlx
    {
        private InteractiveList interactiveList;

        public ICommand GroupClick { get; }

        public static readonly DependencyProperty DetailViewProperty = DependencyProperty.Register("DetailView", typeof(Control), typeof(MasterDetailView), new PropertyMetadata(null, (d, e) => (d as MasterDetailView).ControlChanges.OnNext(e.NewValue as Control)));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register("DataConverter", typeof(IValueConverter), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty DataKeyProperty = DependencyProperty.Register("DataKey", typeof(string), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register("Group", typeof(string), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty UseDataContextProperty = DependencyProperty.Register("UseDataContext", typeof(bool), typeof(MasterDetailView), new PropertyMetadata(false, Changed));

        #region properties

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

        public string Group
        {
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        #endregion properties

        static MasterDetailView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailView), new FrameworkPropertyMetadata(typeof(MasterDetailView)));
        }

        public MasterDetailView()
        {
        }

        public override void OnApplyTemplate()
        {
            var selector = this.Template.Resources["propertytemplateSelector"] as DataTemplateSelector;
            Content ??= new ContentControl { ContentTemplateSelector = selector };

            interactiveList = this.GetTemplateChild("Main_InteractiveList") as InteractiveList;

            //this.SelectChanges<string>(nameof(Group)).Subscribe(a =>
            //{
            //    interactiveList.Group = a;
            //});

            var objects = SelectChanges<IEnumerable>(nameof(ItemsSource))
                .Where(a => a != null)
                .Select(its => its.MakeObservable())
                .Switch();

            var (collectionViewModel, disposable) =
              (Group == null) ?
                 InteractiveCollectionFactory.Build(
                     objects,
                     getkeyObservable: Observable.Return(new Func<object, object>(a => Guid.NewGuid())),
                     isCheckableObervable: Observable.Return(false),
                     filter: Observable.Return(new DefaultFilter()))
             : InteractiveCollectionFactory.BuildGroup(
                     objects,
                     getkeyObservable: Observable.Return(new Func<object, object>(a => Guid.NewGuid())),
                     filter: Observable.Return(new DefaultFilter()),
                     groupParameter: this.SelectChanges<string>(nameof(Group)));
            //isCheckableObervable: Observable.Return(false),

            interactiveList.InteractiveCollectionBase = collectionViewModel;

            collectionViewModel.Items.MakeObservable().Subscribe(a =>
            {
            });

            collectionViewModel.SelectSelected()
                .CombineLatest(
                SelectChanges<IValueConverter>(nameof(DataConverter)).StartWith(default(IValueConverter)),
                SelectChanges<string>(nameof(DataKey)).StartWith(default(string)), (a, b, c) => (a, b, c))
                .ObserveOnDispatcher()
                .Subscribe(a =>
                {
                    var c = a.a;
                    if (a.b != null)
                        c = a.b.Convert(c, default, default, default);
                    if (a.c != null)
                        c = UtilityHelper.PropertyHelper.GetPropertyValue<object>(c, a.c);
                    SetContent(Content, c);
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