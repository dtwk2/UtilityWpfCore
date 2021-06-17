using DynamicData.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityHelper;
using UtilityHelper.NonGeneric;
using UtilityHelperEx;

namespace UtilityWpf.Attached
{
    public partial class ItemsControlEx : ItemsControl
    {
        #region AlternateTemplate

        public static readonly DependencyProperty AlternateTemplateProperty = DependencyProperty.RegisterAttached("AlternateTemplate", typeof(DataTemplate), typeof(ItemsControlEx), new PropertyMetadata(null, AlternateTemplateChange));

        public static object GetAlternateTemplate(DependencyObject d)
        {
            return d.GetValue(AlternateTemplateProperty);
        }

        public static void SetAlternateTemplate(DependencyObject d, DataTemplate value)
        {
            d.SetValue(AlternateTemplateProperty, value);
        }

        private static void AlternateTemplateChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
            {
                DataTemplate originalTemplate = itemsControl.ItemTemplate;
                _ = itemsControl.Items.ObserveCollectionChanges().Select(a => itemsControl.Items.Count)
                    .Subscribe(count =>
                    {
                        if (count == 1)
                        {
                            originalTemplate = itemsControl.ItemTemplate;
                            itemsControl.ItemTemplate = GetAlternateTemplate(d) as DataTemplate;
                        }
                        else
                            itemsControl.ItemTemplate = originalTemplate;
                    });
            }
        }

        #endregion AlternateTemplate

        #region NewItem

        public static readonly DependencyProperty NewItemProperty = DependencyProperty.RegisterAttached("NewItem", typeof(object), typeof(ItemsControlEx), new PropertyMetadata(null, NewItemChange));

        public static object GetNewItem(DependencyObject d)
        {
            return d.GetValue(NewItemProperty);
        }

        public static void SetNewItem(DependencyObject d, object value)
        {
            d.SetValue(NewItemProperty, value);
        }

        private static void NewItemChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var x = (d as ItemsControl).Items?.Cast<object>()?.ToObservableCollection() ?? new ObservableCollection<object>();
            x.Add(e.NewValue);
            Application.Current.Dispatcher.InvokeAsync(() => (d as ItemsControl).ItemsSource = x, System.Windows.Threading.DispatcherPriority.Background, default);
        }

        #endregion NewItem

        #region Variable

        public static string GetVariable(DependencyObject d)
        {
            return (string)d.GetValue(VariableProperty);
        }

        public static void SetVariable(DependencyObject d, object value)
        {
            d.SetValue(VariableProperty, value);
        }

        public static readonly DependencyProperty VariableProperty = DependencyProperty.RegisterAttached("Variable", typeof(string), typeof(ItemsControlEx), new PropertyMetadata(null, VariableChanged));

        private static void VariableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl control = d as ItemsControl;
            string arg = (string)e.NewValue;
            if (control.ItemsSource != null)
                if (control.ItemsSource?.Count() > 0)
                    control.ItemsSource = control.ItemsSource.GetPropertyValues<object>(arg);
        }

        #endregion Variable

        #region ItemsSourcEx

        public static IEnumerable GetItemsSourceEx(DependencyObject d)
        {
            return (IEnumerable)d.GetValue(ItemsSourceExProperty);
        }

        public static void SetItemsSourceEx(DependencyObject d, object value)
        {
            d.SetValue(ItemsSourceExProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceExProperty = DependencyProperty.RegisterAttached("ItemsSourceEx", typeof(IEnumerable), typeof(ItemsControlEx), new PropertyMetadata(null, ItemsSourceExChanged));

        private static void ItemsSourceExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl control = d as ItemsControl;
            IEnumerable arg = (IEnumerable)e.NewValue;
            if (arg.Count() > 0)
                Application.Current.Dispatcher.InvokeAsync(() =>
                control.SetValue(ItemsSourceProperty, arg.GetPropertyValues<object>((string)control.GetValue(VariableProperty)).Cast<IEnumerable<object>>().SelectMany(_s => _s)),
                    System.Windows.Threading.DispatcherPriority.Background, default);
            ;
        }

        #endregion ItemsSourcEx

        #region Filter

        public static string GetFilter(DependencyObject d)
        {
            return (string)d.GetValue(FilterProperty);
        }

        public static void SetFilter(DependencyObject d, object value)
        {
            d.SetValue(FilterProperty, value);
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.RegisterAttached("Filter", typeof(string), typeof(ItemsControlEx), new PropertyMetadata(null, FilterChanged));

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl control = d as ItemsControl;
            if (control.ItemsSource != null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(control.ItemsSource);
                view.Filter = (a) => a.GetType().GetProperties().Where(_ => _.PropertyType == e.NewValue.GetType()).Select(_ => _.GetValue(a)).Any(_ => ((string)_).Contains((string)e.NewValue));
            }
        }

        #endregion Filter

        //private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
        //    CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
        //}

        //      CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
        //      view.Filter = UserFilter;
        //}

        //  private bool UserFilter(object item)
        //  {
        //      if (String.IsNullOrEmpty(txtFilter.Text))
        //          return true;
        //      else
        //          return ((item as User).Name.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        //  }

        //  private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //  {
        //      CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
        //  }

        // public static EventHandler OnValueChanged { get; private set; }
        //    var property = (string)e.NewValue;

        //    ItemsControl i = (d as ItemsControl);
        //    WeakReference wr = new WeakReference(i);
        //    PropertyChangeNotifier notifier = new PropertyChangeNotifier(i, nameof(ItemsControl.ItemsSourceEx));
        //    notifier.ValueChanged += new EventHandler(OnValueChanged);
        //    i = null;
        //    GC.Collect();
        //    bool isAlive = wr.IsAlive;

        //    //(d as PropertyListControl).PropertyChanges.OnNext((string)e.NewValue);
        //    if ((d as ItemsControl).ItemsSourceEx.First().GetType().GetProperties().SingleOrDefault(a => a.Name == _).PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
        //        ItemsSourceExSubject.OnNext(ItemsSourceEx.GetPropertyValues<IEnumerable<object>>(_).SelectMany(a => a));
        //    else
        //        ItemsSourceExSubject.OnNext(ItemsSourceEx.GetPropertyValues<object>(_));
        //});
        //}
        //ISubject<string> PropertyChanges = new Subject<string>();
        //public PropertyListControl()
        //{
        //    //Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/Themes/PropertyControl.xaml", System.UriKind.Relative);
        //    //ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
        //    //Style = resourceDictionary["PropertyControlStyle"] as Style;

        //}
    }
}