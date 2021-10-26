using HistoryControlLib.ViewModels.Base;
using Suggest.FileSystem.Service;
using Suggest.Infrastructure.Interfaces;

namespace Utility.FileSystem.Transfer.Demo {
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

      public Suggestion[] Suggestions { get; } = new Suggestion[] {
            new Suggestion("ddfsdf")
         };

        public IAsyncSuggest Suggest { get; set; }
    }


}
