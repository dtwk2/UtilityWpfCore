using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class PageNavigatorControl : Control
    {
        private SizeControl SizeControl;

        private NavigatorControl NavigatorControl;

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PageNavigatorControl), new PropertyMetadata(null, ItemsSourceChanged));

        public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register("FilteredItems", typeof(IEnumerable), typeof(PageNavigatorControl));

        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(object), typeof(PageNavigatorControl), new PropertyMetadata(null, PageSizeChanged));

        //public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(int), typeof(PageNavigatorControl), new PropertyMetadata(1, OutputChanged));

        public override void OnApplyTemplate()
        {
            SizeControl = this.GetTemplateChild("SizeControl") as SizeControl;
            NavigatorControl = this.GetTemplateChild("NavigatorControl") as NavigatorControl;
            ControlTemplateChanges.OnNext((SizeControl != null) && NavigatorControl != null);
            PageSizeChanges.OnNext(SizeControl.Size);
            NavigatorControl.SelectedIndex += NavigatorControl_SelectedIndex;
            SizeControl.SelectedSizeChanged += SizeControl_SelectedSizeChanged;
        }

        private ISubject<PageRequest> pageRequests = new Subject<PageRequest>();

        private void NavigatorControl_SelectedIndex(object sender, RoutedEventArgs e)
        {
            pageRequests.OnNext(new PageRequest((e as NavigatorControl.SelectedIndexRoutedEventArgs).Index, (int)this.GetValue(PageSizeProperty)));
        }

        private void SizeControl_SelectedSizeChanged(object sender, RoutedEventArgs e)
        {
            pageRequests.OnNext(new PageRequest(NavigatorControl.Current, (e as SizeControl.SelectedSizeChangedRoutedEventArgs).Size));
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public IEnumerable FilteredItems
        {
            get { return (IEnumerable)GetValue(FilteredItemsProperty); }
            set { SetValue(FilteredItemsProperty, value); }
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            (d as PageNavigatorControl).ItemsSourceChanges.OnNext((IEnumerable)e.NewValue);
        }


        private static void PageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PageNavigatorControl).PageSizeChanges.OnNext((int)e.NewValue);
        }

        private ISubject<IEnumerable> ItemsSourceChanges = new Subject<IEnumerable>();

        private ISubject<int> PageSizeChanges = new Subject<int>();
        private ISubject<bool> ControlTemplateChanges = new Subject<bool>();

        static PageNavigatorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageNavigatorControl), new FrameworkPropertyMetadata(typeof(PageNavigatorControl)));
        }

        public PageNavigatorControl()
        {

            var obs2 = new Subject<PageRequest>();

            var obs = pageRequests.StartWith(new PageRequest(1, 20));

            obs.Subscribe(_ =>
                                                                     {
                                                                     });

            ItemsSourceChanges.Subscribe(_ =>
            {
            });

            var filteredPaginatedVM = new UtilityWpf.ViewModel.FilteredPaginatedViewModel<object>(ItemsSourceChanges.Select(_ => _.Cast<object>().ToObservable()).Switch().ToObservableChangeSet(),
           obs,
           obs.Select(_ => new Func<object, bool>(a => true)),
           null);

            this.Dispatcher.InvokeAsync(() =>
            FilteredItems = filteredPaginatedVM.Items, System.Windows.Threading.DispatcherPriority.Background);

            filteredPaginatedVM.WhenAnyValue(a=>a.PageResponse)
                .CombineLatest(ControlTemplateChanges, (a, b) => b ? a : null)
                .Where(_ => _ != null)
                .Subscribe(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                        {
                            SizeControl.TotalSize = _.Page;
                            SizeControl.Size = _.PageSize;
                            NavigatorControl.Size = _.Pages;
                            NavigatorControl.Current = _.TotalSize;
                        });
                });
        }
    }
}

//    public class PageNavigatorControl()
//        {
//        var obs = (CurrentPageSubject).DistinctUntilChanged().CombineLatest(PageSizeSubject, (a, b) =>
//        new { page = a, size = b });

//    var Output = (NextCommand as ReactiveCommand)
//        .WithLatestFrom(obs, (a, b) => new PageRequest(b.page + 1, b.size))
//        .Merge((PreviousCommand as ReactiveCommand)
//        .WithLatestFrom(obs, (a, b) => new PageRequest(b.page - 1, b.size)))
//     /*   .StartWith(new PageRequest(1, 25))*/.ToReactiveProperty();

//    Output.Subscribe(_ =>
//        {
//           this.Dispatcher.InvokeAsync(() => PageRequest = _, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
//            //PageRequest = _;
//        });
//}