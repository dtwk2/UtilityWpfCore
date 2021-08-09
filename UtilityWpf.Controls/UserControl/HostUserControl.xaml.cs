using MoreLinq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper.NonGeneric;

namespace UtilityWpf.Controls
{
    using static UtilityWpf.DependencyPropertyFactory<HostUserControl>;

    /// <summary>
    /// Interaction logic for HostUserControl.xaml
    /// </summary>
    public partial class HostUserControl : UserControl
    {
        Subject<Assembly> subject = new();
        public static readonly DependencyProperty AssemblyProperty = Register(a => a.subject, initialValue: Assembly.GetEntryAssembly());

        public static readonly DependencyProperty UserControlsProperty = Register(nameof(UserControls));


        public HostUserControl()
        {
            // UserControls
            InitializeComponent();

            this.DockPanel1.DataContext = this;

            _ = subject
                .Amb(Observable.Return(Assembly).Delay(TimeSpan.FromSeconds(1)))
                .ObserveOnDispatcher()
                .Select(assembly =>
                {
                    var ucs = assembly
                     .GetTypes()
                     .Where(a => typeof(UserControl).IsAssignableFrom(a))
                     .GroupBy(type =>
                     (type.Name.Contains("UserControl") ? type.Name?.Replace("UserControl", string.Empty) :
                     type.Name.Contains("View") ? type.Name?.Replace("View", string.Empty) :
                     type.Name)!)
                     .OrderBy(a => a.Key)
                     .ToDictionaryOnIndex()
                     .ToDictionary(a => a.Key, a => new Func<FrameworkElement?>(() => (FrameworkElement?)Activator.CreateInstance(a.Value)));

                    return ucs;
                })
                .Subscribe(pairs => UserControls = pairs!);


            MainListBox
                .SelectSelectionAddChanges()
                .Where(a => a.Count == 1)
                .Select(a => a.First())
                .OfType<KeyValuePair<string, Func<FrameworkElement?>>>()
                .Subscribe(a =>
            {
                TitleTextBlock.Text = a.Key;
                ContentControl1.Content = a.Value.Invoke();
            });
        }


        public IDictionary<string, Func<FrameworkElement>> UserControls
        {
            get { return (IDictionary<string, Func<FrameworkElement>>)GetValue(UserControlsProperty); }
            set { SetValue(UserControlsProperty, value); }
        }

        public Assembly Assembly
        {
            get { return (Assembly)GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }
    }

    public static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
           .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
          .ToDictionary(a => a.Key, a => a.Value);
    }
}