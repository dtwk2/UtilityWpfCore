using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrowseHistory;
using BrowserHistoryDemoLib.ViewModels;
using ReactiveUI;

namespace Utility.FileSystem.Transfer.Demo.ViewModel {
   internal class FileSelectorViewModel : ReactiveObject {
      public FileSelectorViewModel() {
   
         NavigationViewModel = new NavigationViewModel();
         SuggestViewModel = new SuggestViewModel();

         SuggestViewModel.PropertyChanged += SuggestViewModel_PropertyChanged;
         //HistoryNavigationControl.DataContext = NavigationViewModel;
         //SuggestBox.DataContext = SuggestViewModel;
         NavigationViewModel.PropertyChanged += NavigationViewModel_PropertyChanged;
      }

      private void NavigationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
         if (e.PropertyName == nameof(NavigationViewModel.CurrentItem)) {
            if (SuggestViewModel.Text != (NavigationViewModel.CurrentItem as PathItem).Path)
               SuggestViewModel.Text = (NavigationViewModel.CurrentItem as PathItem).Path;
         }
      }

      private void SuggestViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
         if (e.PropertyName == nameof(SuggestViewModel.Text)) {
            NavigationViewModel.NavigateCommand.Execute(SuggestViewModel.Text);
         }
      }

      public SuggestViewModel SuggestViewModel { get; }

      public NavigationViewModel NavigationViewModel { get; }
   }
}
