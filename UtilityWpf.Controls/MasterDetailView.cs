using System;
using System.Collections;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using UtilityHelperEx;
using UtilityWpf.Controls.Infrastructure;

namespace UtilityWpf.Controls
{
    public class MasterDetailView : ContentControlx
    {
        protected Subject<string> GroupNameChanges = new();
        protected Subject<string> NameChanges = new();

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(MasterDetailView), new PropertyMetadata("Id", Changed));
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty OutputViewProperty = DependencyProperty.Register("DetailView", typeof(Control), typeof(MasterDetailView), new PropertyMetadata(null));
        public static readonly DependencyProperty PropertyGroupDescriptionProperty = DependencyProperty.Register("PropertyGroupDescription", typeof(PropertyGroupDescription), typeof(MasterDetailView), new PropertyMetadata(null, Changed));
        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register("DataConverter", typeof(IValueConverter), typeof(MasterDetailView), new PropertyMetadata(null, Changed));

        #region properties

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public PropertyGroupDescription PropertyGroupDescription
        {
            get { return (PropertyGroupDescription)GetValue(PropertyGroupDescriptionProperty); }
            set { SetValue(PropertyGroupDescriptionProperty, value); }
        }

        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        #endregion properties

        static MasterDetailView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetailView), new FrameworkPropertyMetadata(typeof(MasterDetailView)));
        }

        public MasterDetailView()
        {
            var outputChanges = SelectPropertyChanges(nameof(Output)).Where(obj => obj != null);

            //Dictionary<Type, object> dict = new Dictionary<Type, object>();
            outputChanges
                .CombineLatest(SelectPropertyChanges<string>(nameof(Id)).StartWith(Id))
                .Select(vm =>
                {
                    var type = vm.First.GetType();
                    var properties = type.GetProperties().ToDictionary(a => a.Name, a => a);
                    if (properties.ContainsKey(vm.Second))
                        return properties[vm.Second].GetValue(vm.First);
                    return properties.Where(a => a.Value.PropertyType == typeof(string)).FirstOrDefault().Value?.GetValue(vm.First);
                })
                .WhereNotNull()
                .Cast<string>()
                .Subscribe(NameChanges);

            outputChanges
                        .CombineLatest(SelectPropertyChanges<IValueConverter>(nameof(DataConverter)).StartWith(default(IValueConverter)))  
                        .ObserveOnDispatcher()
                        .Subscribe(collConv => Dispatcher.InvokeAsync(() =>
                        {
                            Convert(collConv.First, collConv.Second, (items, conv) => conv.Convert(collConv.First, null, null, null) as IEnumerable);
                        }));

            SelectPropertyChanges<PropertyGroupDescription>()
                .StartWith(PropertyGroupDescription)
                .CombineLatest(ControlChanges.Where(c => c.GetType() == typeof(FrameworkElement)).Take(1))
                .Subscribe(a =>
                {
                    if ((a.Second as FrameworkElement)?.FindResource("GroupedItems") is CollectionViewSource collectionViewSource)
                        collectionViewSource.GroupDescriptions.Add(a.First);
                });

            GroupClick = new Command.RelayCommand<string>(a => GroupNameChanges.OnNext(a));

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

            GroupNameChanges
                .CombineLatest(
                  SelectPropertyChanges<PropertyGroupDescription>().StartWith(PropertyGroupDescription),
                SelectPropertyChanges<IValueConverter>(nameof(DataConverter)).StartWith(default(IValueConverter)),
                SelectPropertyChanges<string>(nameof(Id)).StartWith(Id)) 
                .Subscribe(async input =>
                {
                    var (text, groupDescriptor, conv, id) = input;
                    await Dispatcher.InvokeAsync(() =>
                    {
                        var paths = Items.Cast<object>();
                        var prop = paths.First().GetType().GetProperty(groupDescriptor.PropertyName);

                        // property-group-converter
                        var converter = groupDescriptor.Converter;

                        var group = paths.Where(ad =>
                        {
                            bool result = converter != default ?
                                    text
                                            .Equals(converter.Convert(prop.GetValue(ad), null, null, null)) :
                                    text
                                            .Equals(prop.GetValue(ad));
                            return result;
                        })
                        .Select(viewmodel =>
                        new Property.KeyValue(
                            viewmodel.GetType().GetProperty(id).GetValue(viewmodel).ToString(),
                            viewmodel));
                        Convert(group, conv, (items, conv) => conv.Convert(items, null, null, null));
                    });
                });

            ContentTemplateSelector = new PropertyDataTemplateSelector();
        }

        private void Convert(object items, IValueConverter conv, Func<object, IValueConverter, object> func)
        {
            Content = convert(conv, func, items);

            static object convert(IValueConverter conv, Func<object, IValueConverter, object> func, object items)
            {
                return
                      conv == default ?
                      items :
                      func(items, conv);
            }
        }

        public ICommand GroupClick { get; }

        //class TemplateSelector : DataTemplateSelector
        //{
        //    private readonly MasterDetailView masterDetailView;

        //    public TemplateSelector(MasterDetailView masterDetailView)
        //    {
        //        this.masterDetailView = masterDetailView;
        //    }

        //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
        //    {
        //        if (item == null)
        //            return default;
        //        var resource = masterDetailView.Template.Resources;
        //        return (item, container) switch
        //        {
        //            (Control _, { } _) => default,
        //            (IConvertible _, { } frameworkElement) => resource["IConvertiblePropertyTemplate"],
        //            (IDictionary _, { } frameworkElement) => resource["DictionaryTemplate"],
        //            (IEnumerable _, { } frameworkElement) => resource["EnumerableTemplate"],
        //            (Type _, { } frameworkElement) => resource["TypeTemplate"],
        //            (object o, { } frameworkElement) when
        //            Splat.Locator.Current.GetService(typeof(ReactiveUI.IViewLocator)) is ReactiveUI.IViewLocator viewLocator &&
        //            viewLocator.ResolveView(o) != null => resource["ViewModelTemplate"],
        //            (_, FrameworkElement frameworkElement) =>
        //            frameworkElement.TryFindResource(new DataTemplateKey(item.GetType())) ??
        //            Application.Current.TryFindResource(new DataTemplateKey(item.GetType())) ??
        //            resource["DefaultTemplate"],
        //            _ => null
        //        } as DataTemplate;
        //    }

        //}
    }
}