using Bogus;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DynamicData;
using System.Collections.ObjectModel;


namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for ListBoxCriteria.xaml
    /// </summary>
    public partial class ListBoxCriteriaUserControl : UserControl
    {
        private ReadOnlyObservableCollection<PassFail> a;
        private ReadOnlyObservableCollection<PassFail> b;

        public ListBoxCriteriaUserControl()
        {

            Random random = new Random();
            InitializeComponent();
            new Faker<PassFail>()
                .RuleFor(a => a.Key, f => f.Name.FirstName())
                .RuleFor(a => a.Expired, f => f.Random.Bool())
                .GenerateLazy(10000)
                .ToObservable().Zip(Observable.Interval(TimeSpan.FromSeconds(3)), (a, b) => a)
                .ToObservableChangeSet(a => a.Key)
                .Sort(new DefaultComparer<PassFail, string>(new Func<PassFail, string>(a => a.Key)))
                .Top(10)
                .ObserveOnDispatcher()
                .Bind(out a)
                .Subscribe(a =>
                {
                });
            passFail.ItemsSource = a;



            new Faker<PassFail>()
             .RuleFor(a => a.Key, f => "Bob")
             .RuleFor(a => a.Expired, f => f.Random.Bool())
             .GenerateLazy(10000)
             .ToObservable().Zip(Observable.Interval(TimeSpan.FromSeconds(3)), (a, b) => a)
             .ToObservableChangeSet(a => a.Key)
             .Sort(new DefaultComparer<PassFail, string>(new Func<PassFail, string>(a => a.Key)))
             .Top(10)
             .ObserveOnDispatcher()
             .Bind(out b)
             .Subscribe(a =>
             {
             });

            passFail2.ItemsSource = b;
        }


        internal class PassFail
        {
            public string Key { get; set; }

            public bool Expired { get; set; }
        }
   
    }
}