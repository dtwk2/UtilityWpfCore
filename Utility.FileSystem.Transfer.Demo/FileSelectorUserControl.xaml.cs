using BrowseHistory;
using BrowserHistoryDemoLib.ViewModels;
using CachedPathSuggest.Service;
using HistoryControlLib.ViewModels.Base;
using SuggestBoxLib.Infrastructure.Interfaces;
using System.ComponentModel;
using System.Windows.Controls;

namespace Utility.FileSystem.Transfer.Demo
{
    /// <summary>
    /// Interaction logic for FileSelectorUserControl.xaml
    /// </summary>
    public partial class FileSelectorUserControl : UserControl
    {
        public FileSelectorUserControl()
        {
            InitializeComponent();

            NavigationViewModel = new NavigationViewModel();
            SuggestViewModel = new SuggestViewModel();

            SuggestViewModel.PropertyChanged += SuggestViewModel_PropertyChanged;

            HistoryNavigationControl.DataContext = NavigationViewModel;
            SuggestBox.DataContext = SuggestViewModel;

            NavigationViewModel.PropertyChanged += NavigationViewModel_PropertyChanged;
        }

        private void NavigationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NavigationViewModel.CurrentItem))
            {
                if (SuggestViewModel.Text != (NavigationViewModel.CurrentItem as PathItem).Path)
                    SuggestViewModel.Text = (NavigationViewModel.CurrentItem as PathItem).Path;
            }
        }

        private void SuggestViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SuggestViewModel.Text))
            {
                NavigationViewModel.NavigateCommand.Execute(SuggestViewModel.Text);
            }
        }

        public SuggestViewModel SuggestViewModel { get; }

        public NavigationViewModel NavigationViewModel { get; }
    }


    public class SuggestViewModel : BaseViewModel
    {
        private string text;

        public SuggestViewModel()
        {

            Suggest = new DirectoryAsyncSuggest();
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public IAsyncSuggest Suggest { get; set; }
    }


}
