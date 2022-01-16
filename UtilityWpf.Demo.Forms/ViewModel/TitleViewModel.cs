using _ViewModel = Utility.ViewModel.ViewModel;

namespace UtilityWpf.Demo.Forms.ViewModel
{
    public class TitleViewModel : _ViewModel
    {
        private string title = "The Title";
        private string subtitle = "The SubTitle";

        public TitleViewModel() : base("Title")
        {
        }

        public string Title { get => title; set => this.OnPropertyChanged(ref title, value); }
        public string SubTitle { get => subtitle; set => this.OnPropertyChanged(ref subtitle, value); }
    }
}
