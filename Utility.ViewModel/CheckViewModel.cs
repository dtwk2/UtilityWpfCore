namespace Utility.ViewModel
{
    public class CheckViewModel
    {
        public CheckViewModel(string header, bool isChecked)
        {
            Header = header;
            IsChecked = isChecked;
        }

        public CheckViewModel()
        {
        }

        public bool IsChecked { get; set; }

        public string Header { get; init; }


        public override string ToString()
        {
            return $"{Header} {IsChecked}";
        }
    }

    public class CheckContentViewModel : CheckViewModel
    {
        public CheckContentViewModel(object content, string header, bool isChecked) : base(header, isChecked)
        {
            Content = content;
        }

        public CheckContentViewModel()
        {
        }

        public object Content { get; }
    }
}
