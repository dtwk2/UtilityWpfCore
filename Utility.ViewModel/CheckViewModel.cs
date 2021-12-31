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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Header} {IsChecked}";
        }
    }
}