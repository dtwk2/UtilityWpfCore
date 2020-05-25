﻿using System;
using System.Collections;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Input;


namespace UtilityWpf.View
{
    public class MasterDetailView : Controlx
    {


        protected ISubject<string> GroupNameChanges = new Subject<string>();
        protected ISubject<string> NameChanges = new Subject<string>();

        #region properties
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(MasterDetailView), new PropertyMetadata(null, Changed));

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(MasterDetailView), new PropertyMetadata(null, Changed));

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(MasterDetailView), new PropertyMetadata("Id", Changed));

        public Control DetailView
        {
            get { return (Control)GetValue(OutputViewProperty); }
            set { SetValue(OutputViewProperty, value); }
        }

        public static readonly DependencyProperty OutputViewProperty = DependencyProperty.Register("DetailView", typeof(Control), typeof(MasterDetailView), new PropertyMetadata(null));

        public PropertyGroupDescription PropertyGroupDescription
        {
            get { return (PropertyGroupDescription)GetValue(PropertyGroupDescriptionProperty); }
            set { SetValue(PropertyGroupDescriptionProperty, value); }
        }

        public static readonly DependencyProperty PropertyGroupDescriptionProperty = DependencyProperty.Register("PropertyGroupDescription", typeof(PropertyGroupDescription), typeof(MasterDetailView), new PropertyMetadata(null, Changed));

        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register("DataConverter", typeof(IValueConverter), typeof(MasterDetailView), new PropertyMetadata(null, Changed));

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }


        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }
        #endregion properties

        //protected ISubject<PropertyGroupDescription> PropertyGroupDescriptionChanges { get; } = new Subject<PropertyGroupDescription>();

        //public override void OnApplyTemplate()
        //{
        //    this.ControlChanges.OnNext(this.GetTemplateChild("DockPanel") as DockPanel);
        //    this.ControlChanges.OnNext(this.GetTemplateChild("TextBlock1") as TextBlock);
        //}

        static MasterDetailView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailView), new FrameworkPropertyMetadata(typeof(MasterDetailView)));
        }

        public MasterDetailView()
        {
            var outputChanges = SelectChanges(nameof(MasterDetailView.Output)).Where(obj => obj != null);

            //Dictionary<Type, object> dict = new Dictionary<Type, object>();
            outputChanges
                .CombineLatest(SelectChanges<string>(nameof(MasterDetailView.Id)).StartWith(Id), (a, b) => (a, b))
                .Select(vm =>
                {
                    var id = vm.a.GetType().GetProperty(vm.b).GetValue(vm.a).ToString();
                    return id;
                }).Subscribe(NameChanges);

            outputChanges
                        .CombineLatest(SelectChanges<IValueConverter>(nameof(MasterDetailView.DataConverter)).StartWith(default(IValueConverter)), (a, b) => (a, b))
                        .SubscribeOn(TaskPoolScheduler.Default)
                        .ObserveOnDispatcher()
                        .Subscribe(collConv => this.Dispatcher.InvokeAsync(() =>
                        {
                            Convert(collConv.a, collConv.b, (items, conv) => conv.Convert(collConv.a, null, null, null) as IEnumerable);
                        }, System.Windows.Threading.DispatcherPriority.Normal));

            SelectChanges<PropertyGroupDescription>().StartWith(PropertyGroupDescription)
                .CombineLatest(ControlChanges.Where(c => c.GetType() == typeof(DockPanel)).Take(1), (pgd, DockPanel) => (pgd, DockPanel)).Subscribe(_ =>
            {
                if ((_.DockPanel as DockPanel)?.FindResource("GroupedItems") is CollectionViewSource collectionViewSource)
                    collectionViewSource.GroupDescriptions.Add(_.pgd);
            });

            DetailView = DetailView ?? new JsonView();


            GroupClick = new RelayCommand<string>(a => GroupNameChanges.OnNext(a));

            NameChanges
                .Merge(GroupNameChanges)
                .CombineLatest(ControlChanges.Select(c => c as TextBlock).Where(c => c != null),

                (text, textBlock) => (text, textBlock))
                .ObserveOnDispatcher()
                .Subscribe(input =>
                {
                    input.textBlock.Text = input.text;
                    input.textBlock.Visibility = Visibility.Visible;
                    input.textBlock.IsEnabled = true;
                    input.textBlock.IsEnabled = false;
                });

            GroupNameChanges.CombineLatest(
                  SelectChanges<PropertyGroupDescription>().StartWith(PropertyGroupDescription),
                SelectChanges<IValueConverter>(nameof(MasterDetailView.DataConverter)).StartWith(default(IValueConverter)),
                                SelectChanges<string>(nameof(MasterDetailView.Id)).StartWith(Id),
                (text, pg, conv, id) => (text, pg, conv, id))
                //.ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(async input =>
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        var paths = Items.Cast<object>();
                        var prop = paths.First().GetType().GetProperty(input.pg.PropertyName);

                        // property-group-converter
                        var converter = input.pg.Converter;

                        var group = paths.Where(ad =>
                        {
                            bool result = converter != default ?
                                    input.text
                                            .Equals(converter.Convert(prop.GetValue(ad), null, null, null)) :
                                    input.text
                                            .Equals(prop.GetValue(ad));
                            return result;
                        })
                        .Select(viewmodel =>
                        new KeyValue(
                            viewmodel.GetType().GetProperty(input.id).GetValue(viewmodel).ToString(),
                            viewmodel));
                        Convert(group, input.conv, (items, conv) => conv.Convert(items, null, null, null));

                    }, DispatcherPriority.Background);
                });
        }

        private void Convert(object items, IValueConverter conv, Func<object, IValueConverter, object> func)
        {
            if (DetailView is Abstract.IObject oview)
            {
                oview.Object = convert(conv, func, items);
            }
            else if (DetailView is JsonView propertyGrid)
            {
                var xx = convert(conv, func, items);
                if (typeof(IEnumerable).IsAssignableFrom(xx.GetType()))
                {
                    var xs = xx as IEnumerable;
                    propertyGrid.Object = xs;
                }
                else
                    propertyGrid.Object = xx;
            }
            else
                throw new Exception(nameof(DetailView) + " needs to have property OutputView");

            static object convert(IValueConverter conv, Func<object, IValueConverter, object> func, object items)
            {
                return
                      conv == default ?
                      items :
                      func(items, conv);
            }
        }

        public ICommand GroupClick { get; }


        public class KeyValue
        {
            public KeyValue(string key, object value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; }

            public object Value { get; }
        }
    }
}