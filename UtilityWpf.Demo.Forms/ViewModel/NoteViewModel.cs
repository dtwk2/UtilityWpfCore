using ReactiveUI;

namespace UtilityWpf.Demo.Forms.ViewModel
{
    public class NoteViewModel : ReactiveObject
    {
        private string text;

        public NoteViewModel(string text)
        {
            this.text = text;
        }

        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }
    }
}
