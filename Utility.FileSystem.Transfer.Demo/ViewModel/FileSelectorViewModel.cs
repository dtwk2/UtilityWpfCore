//using BrowseHistory;
using BrowserHistoryDemoLib.ViewModels;

//using HistoryControlLib.ViewModels;
using ReactiveUI;
using System.ComponentModel;

namespace Utility.FileSystem.Transfer.Demo.ViewModel
{
    internal class FileSelectorViewModel : ReactiveObject
    {
        public FileSelectorViewModel()
        {
            //var naviHistory = new BrowseHistory<PathItem>();
            //NavigationViewModel = new NavigationViewModel(naviHistory);
            //SuggestViewModel = new SuggestViewModel(naviHistory);

            //SuggestViewModel.PropertyChanged += SuggestViewModel_PropertyChanged;
            ////HistoryNavigationControl.DataContext = NavigationViewModel;
            ////SuggestBox.DataContext = SuggestViewModel;
            //NavigationViewModel.PropertyChanged += NavigationViewModel_PropertyChanged;
        }

        public NavigationViewModel NavigationViewModel { get; }

        public SuggestViewModel SuggestViewModel { get; }

        private void NavigationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(NavigationViewModel.CurrentItem)) {
            //   if (SuggestViewModel.Text != (NavigationViewModel.CurrentItem as PathItem).Path)
            //      SuggestViewModel.Text = (NavigationViewModel.CurrentItem as PathItem).Path;
            //}
        }

        private void SuggestViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(SuggestViewModel.Text)) {
            //   NavigationViewModel.NavigateCommand.Execute(SuggestViewModel.Text);
            //}
        }
    }
}