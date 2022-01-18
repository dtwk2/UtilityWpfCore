using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Utility.ViewModel
{
    public class HeaderedNPC : IReactiveObject
    {
        public HeaderedNPC(string header)
        {
            Header = header;
        }

        public string Header { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging?.Invoke(this, args);
        }
    }
}