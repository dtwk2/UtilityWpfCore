using System.ComponentModel;
using System.Windows;

namespace UtilityWpf.DemoApp
{
    public enum Gender
    {
        Female,
        Male,
        Unknown
    }

    public class Character : INotifyPropertyChanged
    {
        private string _first = string.Empty;

        public string First
        {
            get { return _first; }
            set
            {
                _first = value;
                RaisePropertyChanged("First");
            }
        }

        private string _last = string.Empty;

        public string Last
        {
            get { return _last; }
            set
            {
                _last = value;
                RaisePropertyChanged("Last");
            }
        }

        private string _image = null;

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged("Image");
            }
        }

        private int _age = 0;

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                RaisePropertyChanged("Age");
            }
        }

        private Gender _gender = Gender.Unknown;

        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                RaisePropertyChanged("Gender");
            }
        }

        private Point _location = new Point();

        public Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged("Location");
            }
        }

        public override string ToString()
        {
            return _first + " " + _last;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged Members
    }
}