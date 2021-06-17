using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.DemoApp.ViewModel;
using UtilityWpf.ViewModel;

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class ViewModelsUserControl : ReactiveUserControl<ViewModelsViewModel>
    {
        public ViewModelsUserControl()
        {
            InitializeComponent();
            this
              .WhenActivated(
                  disposables =>
                  {
                      Locator.CurrentMutable.Register(()=>new NumberBoxViewModel());
                      Locator.CurrentMutable.Register(() => new NumberBoxViewModel());
                      Locator.CurrentMutable.Register(()=> new DataRangeViewModel());

                      (Resources["GroupedSamples"] as CollectionViewSource).Source = (ViewModel ??= new ViewModelsViewModel()).Collection;

                      ViewModel.SelectedItem = samplesListBox.SelectedItem;

                      this
                      .OneWayBind(ViewModel,
                      x => x.SelectedItem,
                      x => x.sampleViewModelViewHost.ViewModel,
                      x => x == null ? null : ((KeyValuePair<string, KeyValuePair<string, object>>)x).Value.Value)
                      .DisposeWith(disposables);

                      this.samplesListBox.SelectionChanged += SamplesListBox_SelectionChanged;
                  });
        }

        private void SamplesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (ViewModel).SelectedItem = e.AddedItems.Cast<object>().First();
        }

        private void Button_Home_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
