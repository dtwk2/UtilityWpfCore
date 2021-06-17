using ReactiveUI;

namespace UtilityWpf.Interactive.Demo
{
    /// <summary>
    /// Interaction logic for ViewModelAssemblyView.xaml
    /// </summary>
    public partial class ViewModelAssemblyView :ReactiveUserControl<ViewModelAssemblyViewModel>
    {
        public ViewModelAssemblyView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.MainMasterDetailView.ItemsSource = ViewModel.Collection;
            });
        }

    }
}